import css from './files-form.module.css'
import FormFilePath from '../components/form-filepath'
import FormFileBranch from '../components/form-filebranch'
import { useEffect, useState } from 'react'
import { useList } from 'hooks/useList'


function FilesForm() {

    const [filesInfo] = useState({
        name: "Группа 1",
        type: {
            "name": "Group",
            "id": 3
        },
        path: [
            {
                "name": "Корень1",
                "guid": "1ecf9a96-27fc-4d26-9a96-d9b746e35513",
                "type": {
                    "name": "Folder",
                    "id": 1
                }
            },
            {
                "name": "Корень2",
                "guid": "1ecf9a96-27fc-4d26-9a96-d9b746e35523",
                "type": {
                    "name": "Folder",
                    "id": 1
                }
            },
            {
                "name": "Корень3",
                "guid": "1ecf9a96-27fc-4d26-9a96-d9b746e35533",
                "type": {
                    "name": "Folder",
                    "id": 1
                }
            },
            {
                "name": "Корень4",
                "guid": "1ecf9a96-27fc-4d26-9a96-d9b746e35543",
                "type": {
                    "name": "Folder",
                    "id": 1
                }
            },
            {
                "name": "Группа 1",
                "guid": "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27",
                "type": {
                    "name": "Group",
                    "id": 3
                }
            }
        ],
        realPath: [
            "1ecf9a96-27fc-4d26-9a96-d9b746e35523",
            "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27"
        ], // dn
        guid: "7989dbf3-35a0-4efa-9a2f-5fe40e4b7c27",
        children: [
            {
                "name": "Графический дизайн",
                "type": {
                    "name": "Subject",
                    "id": 4
                },
                "guid": "2b322bfd-21a1-4b23-81e5-16294f5b4651",
                "creationTime": "2022-11-16T13:23:18.893789",
                "group": "Группа 1",
                "teacher": 3,
                "description": null
            },
            {
                "name": "sdfgosdifhjfgsdifgguiogsdhjfg",
                "type": {
                    "name": "Subject",
                    "id": 4
                },
                "guid": "2b322bfd-21a1-4b23-81e5-16294f5b4652",
                "creationTime": "2022-11-16T13:23:18.893789",
                "group": "Группа 1",
                "teacher": 3,
                "description": null
            },
            {
                "name": "Графический дизайн2",
                "type": {
                    "name": "Subject",
                    "id": 4
                },
                "guid": "2b322bfd-21a1-4b23-81e5-16294f5b4653",
                "creationTime": "2022-11-16T13:23:18.893789",
                "group": "Группа 1",
                "teacher": 3,
                "description": null
            },
            
        ],
        "creationTime": "2022-11-16T13:22:19.386398",
        "teacher": 2,
        "data": {
            "subjects": [
                {
                    "id": "2b322bfd-21a1-4b23-81e5-16294f5b4651",
                    "name": "Графический дизайн"
                }
            ],
            "students": []
        }
        }
    )
    const [branchItems, branchItemsAtions, branchItemsStateActions] = useList((current)=>console.log(current))
   
    useEffect(()=>{
        branchItemsAtions.setItems([
            {
                "name": "Графический дизайн",
                "type": {
                    "name": "Subject",
                    "id": 4
                },
                "guid": "2b322bfd-21a1-4b23-81e5-16294f5b4651",
                "creationTime": "2022-11-16T13:23:18.893789",
                "group": "Группа 1",
                "teacher": 3,
                "description": null
            },
            {
                "name": "sdfgosdifhjfgsdifgguiogsdhjfg",
                "type": {
                    "name": "Subject",
                    "id": 4
                },
                "guid": "2b322bfd-21a1-4b23-81e5-16294f5b4652",
                "creationTime": "2022-11-16T13:23:18.893789",
                "group": "Группа 1",
                "teacher": 3,
                "description": null
            },
            {
                "name": "Графический дизайн2",
                "type": {
                    "name": "Subject",
                    "id": 4
                },
                "guid": "2b322bfd-21a1-4b23-81e5-16294f5b4653",
                "creationTime": "2022-11-16T13:23:18.893789",
                "group": "Группа 1",
                "teacher": 3,
                "description": null
            },
            
        ])
        // !!!
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return (
        <div className={css.block}>
            <div className={css.header}>
                <FormFilePath path={filesInfo.path}/>
                <p className={css.username}>Username</p>
            </div>
            <div className={css.body}>
                <FormFileBranch items={branchItems} actions={branchItemsAtions} state={branchItemsStateActions}/>
                <div></div>
            </div>

            
        </div>
    )
}

export default FilesForm
