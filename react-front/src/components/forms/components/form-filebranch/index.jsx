import css from './form-filebranch.module.css'
import FormLoader from '../form-loader'
import { faPen, faTrash, faCirclePlus, faCheck } from '@fortawesome/free-solid-svg-icons'
import { useContext } from 'react'
import { GlobalUIContext } from 'contexts/GlobalUIContext'
import CreateForm from 'components/forms/create-form'
import Types from './components/fileBranchTypes'
import { useNavigate } from 'react-router-dom'
import FilebranchNotFound from './components/filebranchNotFound'

function FormFileBranch({ current, items, actions, state, loading, store, requests, setFilesInfo }) {
    const navigate = useNavigate()
    const { remove } = requests

    const { popup, menu } = useContext(GlobalUIContext)

    const RenameItem = async (current) => {
        state.editModeOff(current.guid)
    }

    const EditModeItemOn = (current) => {
        state.editModeOn(current.guid)
        store.storeProp(current.guid, 'name')
    }

    const DeleteItem = async (current) => {
        await remove(current.guid)
    }

    const MainMenuOpen = (evt) => {
        const access = current.access.filter(item => item !== 'Work')

        if (!loading && access.length){
            menu.open()
            menu.setCurrent(current)
            menu.setItems([{ text: 'Создать', icon: faCirclePlus, action: PopupCreateFormOpen }])
            menu.setCoords(evt.clientX, evt.clientY)
        }
    }

    const PopupCreateFormOpen = (current) => {
        popup.open()
        popup.setCurrent(current)
        popup.setContent(<CreateForm setFilesInfo={setFilesInfo} />)
    }

    const ItemsMenuOpen = (evt, data, editMode) => {
        menu.open()

        const items = editMode
            ? [{ text: 'Сохранить', icon: faCheck, action: RenameItem }]
            : [
                { text: 'Переименовать', icon: faPen, action: EditModeItemOn },
                { text: 'Удалить', icon: faTrash, action: DeleteItem },
            ]
        menu.setItems(items)
        menu.setCurrent(data)
        menu.setCoords(evt.clientX, evt.clientY)
        evt.stopPropagation()
    }




    return <div className={css.block}
        onContextMenu={(evt) => {
            evt.preventDefault()
            MainMenuOpen(evt)
        }}>
        <FormLoader loading={loading}>
            {items.map(
                (item) => {
                    const Elem = Types[item.value.type.name] || FilebranchNotFound
                    return <Elem
                        key={'FBI' + item.value.guid}
                        {...item}
                        navigateTo={(id)=> navigate('/files/'+id)}
                        actions={actions.getItem(item.value.guid)}
                        onMenu={(evt, data, editMode) => {
                            evt.preventDefault()
                            ItemsMenuOpen(evt, data, editMode)
                        }}
                    />
                }
            )}
        </FormLoader>
    </div>
}

export default FormFileBranch
