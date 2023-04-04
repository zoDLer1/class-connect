import css from './files-form.module.css'
import FormFilePath from '../components/form-filepath'
import FormFileBranch from '../components/form-filebranch'
import { useState } from 'react'
import { useList } from 'hooks/useList'
import FilesService from 'services/filesService'
import user from 'store/user'
import { GlobalUIContext } from 'contexts/GlobalUIContext'
import { useRequest } from 'hooks/useRequest'
import FormFileInfo from '../components/formFileInfo'
import { useParams } from 'react-router-dom'
import { useEffect, useContext } from 'react'
import { useNavigate } from 'react-router-dom'



function FilesForm() {

    const { alert } = useContext(GlobalUIContext)


    const navigate = useNavigate()

    const { id } = useParams()

    const [parentFileInfo, setParentFileInfo] = useState()

    const FOLDER_NOT_FOUND = () => {
        alert.show("Папка не найдена")
        navigate('/files/' + user.data.folder)
    }

    const [remove] = useRequest(
        async (id) => await FilesService.remove(id),
        {
            200: (response) => branchItemsAtions.removeItem(response.config.params.id)
        }
    )

    const [rename] = useRequest(
        async ({ id, name }) => await FilesService.rename(id, name),
        {
            200: (response) => branchStoreActions.commit(response.config.params.id),
            400: (response) => branchStoreActions.reject(JSON.parse(response.config.data).params.id, 'name'),
            0: (response) => branchStoreActions.reject(JSON.parse(response.config.data).params.id, 'name')

        }
    )

    const [setFolder, isLoading] = useRequest(
        async (id) => await FilesService.get_folder(id),
        {
            200: (response) => {
                setFilesInfo(response.data)
            },
            404: FOLDER_NOT_FOUND,
            403: (response) => {
                if (response.config.params.id === user.data.folder) {
                    user.set_user_data({ ...user.data, folder: null })
                    navigate('/rootNotFound')
                }
                return FOLDER_NOT_FOUND()
            },
            400: FOLDER_NOT_FOUND
        }
    )
    const updateInfo = async () => {
        await setFolder(id)
    }
    const setFilesInfo = (data) => {
        const { children, path, ...fileInfo } = data
        setFilePath(path)
        const guid = selectedItem?.guid
        branchItemsAtions.setItems(children)
        
        if (guid){
            branchItemsStateActions.selectedStateOn(guid)
        }
        setParentFileInfo(fileInfo)
    }


    useEffect(() => {

        const renderFolder = async () => {
            await setFolder(id)
        }
        if ('null' !== id) {
            renderFolder()
        }
        else {
            navigate('/rootNotFound')
        }


    }, [id]);



    const [filePath, setFilePath] = useState([
        {
            "name": "",
            "guid": "1",
            "type": {
                "name": "Folder",
                "id": 1
            }
        },
        {
            "name": "",
            "guid": "2",
            "type": {
                "name": "Folder",
                "id": 1
            }
        },
        {
            "name": "",
            "guid": "3",
            "type": {
                "name": "Folder",
                "id": 1
            }
        }
    ],
    )
    const [branchItems, branchItemsAtions, branchItemsStateActions, branchStoreActions, selectedItem] = useList((current) => current.stored.name !== current.value.name ? rename({ id: current.value.guid, name: current.value.name }) : branchStoreActions.reject(current.value.guid, 'name'))


    return (
        <div className={css.block}>
        
            <div className={css.header}>
                <FormFilePath loading={isLoading} path={filePath} />
                <div className={css.user_info}>
                    <p className={[css.role, css[`role--${user.data.role.toLowerCase()}`]].join(' ')}>{user.data.role}</p>
                    <p className={css.username}>{user.data.name} {user.data.surname}</p>
                </div>

            </div>
            <div className={css.body}>
                <FormFileBranch
                    setFilesInfo={setFilesInfo}
                    current={parentFileInfo}
                    loading={isLoading}
                    store={branchStoreActions}
                    items={branchItems}
                    actions={branchItemsAtions}
                    state={branchItemsStateActions}
                    requests={{ remove }}
                />
                <FormFileInfo update={updateInfo} {...selectedItem || parentFileInfo} />
            </div>
        </div>
    )
}

export default FilesForm
