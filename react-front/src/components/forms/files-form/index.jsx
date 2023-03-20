import css from './files-form.module.css'
import FormFilePath from '../components/form-filepath'
import FormFileBranch from '../components/form-filebranch'
import { useState } from 'react'
import { useList } from 'hooks/useList'
import FilesService from 'services/filesService'
import user from 'store/user'
import { useLoading } from 'hooks/useLoading'




function FilesForm() {


    const changeFolder = async (id) => {
        const response = await FilesService.get_folder(id)
        setFilePath(response.data.path)
        branchItemsAtions.setItems(response.data.children)
    }

    const { isLoading } = useLoading(
        async () => await changeFolder(user.data.folder)
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
    const [branchItems, branchItemsAtions, branchItemsStateActions] = useList((current) => console.log(current))



    return (
        <div className={css.block}>
            <div className={css.header}>
                <FormFilePath loading={isLoading} path={filePath} />
                <p className={css.username}>Username</p>
            </div>
            <div className={css.body}>
                <FormFileBranch loading={isLoading} items={branchItems} actions={branchItemsAtions} state={branchItemsStateActions} />
                <div></div>
            </div>


        </div>
    )
}

export default FilesForm
