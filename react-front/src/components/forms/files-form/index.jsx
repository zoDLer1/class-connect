import css from './files-form.module.css'
import FormFilePath from '../components/form-filepath'
import FormFileBranch from '../components/form-filebranch'
import { useState } from 'react'
import { useList } from 'hooks/useList'
import FilesService from 'services/filesService'
import user from 'store/user'
import { useLoading } from 'hooks/useLoading'




function FilesForm() {

    const [fileInfo, setFileInfo] = useState()

    const setFolder = async (id) => {
        const response = await FilesService.get_folder(id)
        const { children, path, ...fileInfo } = response.data
        setFilePath(path)
        branchItemsAtions.setItems(children)
        setFileInfo(fileInfo)
    }

    const { isLoading } = useLoading(
        async () => await setFolder(user.data.folder)
    )


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
    const [branchItems, branchItemsAtions, branchItemsStateActions, branchStoreActions] = useList((current) => console.log(current))


    return (
        <div className={css.block}>
            <div className={css.header}>
                <FormFilePath setFolder={setFolder} loading={isLoading} path={filePath} />
                <div className={css.user_info}>
                    <p className={[css.role, css[`role--${user.data.role.toLowerCase()}`]].join(' ')}>{user.data.role}</p>
                    <p className={css.username}>{user.data.name} {user.data.surname}</p>
                </div>
                
            </div>
            <div className={css.body}>
                <FormFileBranch setFolder={setFolder} current={fileInfo} loading={isLoading} store={branchStoreActions} items={branchItems} actions={branchItemsAtions} state={branchItemsStateActions} />
                <div></div>
            </div>
        </div>
    )
}

export default FilesForm
