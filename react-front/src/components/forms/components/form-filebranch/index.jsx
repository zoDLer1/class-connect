import css from './form-filebranch.module.css'
import FileBranchItem from './filebranch-item'
import { useMenu } from 'hooks/useMenu'
import Menu from 'components/UI/Menu'
import FormLoader from '../form-loader'
import FilesService from 'services/filesService'
import { faPen, faTrash, faCirclePlus, faCheck } from '@fortawesome/free-solid-svg-icons'
import { useContext } from 'react'
import { GlobalUIContext } from 'contexts/GlobalUIContext'
import CreateForm from 'components/forms/create-form'
import { useRequest } from 'hooks/useRequest'


function FormFileBranch({ current, items, actions, state, loading, store, setFolder }) {

    const [remove] = useRequest(
        async (id) => await FilesService.remove(id),
        {
            200: (response) => actions.removeItem(response.config.params.id)
        }
    )
    const [rename] = useRequest(
        async ({ id, name }) => await FilesService.rename(id, name),
        {
            200: (response) => store.commit(response.config.params.id),
            400: (response) => store.reject(JSON.parse(response.config.data).params.id, 'name'),
            0: (response) => store.reject(JSON.parse(response.config.data).params.id, 'name')
            
        }
    )



    const { popup } = useContext(GlobalUIContext)



    const [menu, menuActions] = useMenu([
        { text: 'Создать', icon: faCirclePlus, action: (current) => { popup.open(); popup.setCurrent(current); popup.setContent(<CreateForm setFolder={setFolder} />) } }
    ])
    const [itemMenu, itemMenuActions] = useMenu([
        { text: 'Переименовать', icon: faPen, action: (current) => {state.editModeOn(current.guid); store.storeProp(current.guid, 'name');} },
        { text: 'Удалить', icon: faTrash, action: async (current) => await remove(current.guid) },
    ])




    return (
        <>
            <div className={css.block} onContextMenu={(evt) => {
                evt.preventDefault()
                menuActions.open()
                menuActions.setCurrent(current)
                menuActions.setCoords(evt.clientX, evt.clientY)
            }}>
                <FormLoader loading={loading}>
                    {items.map((item) => <FileBranchItem
                        key={'FBI' + item.value.guid}
                        setFolder={setFolder}
                        {...item}
                        actions={actions.getItem(item.value.guid)}
                        onMenu={
                            (evt, data, editMode) => {
                                evt.preventDefault()
                                itemMenuActions.open()
                                if (editMode) {
                                    itemMenuActions.changeItems([{
                                        text: 'Сохранить', 
                                        icon: faCheck, 
                                        action: async (current) => {
                                            state.editModeOff(current.guid)
                                            await rename({id: current.guid, name: current.name})
                                        }
                                    }])
                                }
                                else {
                                    itemMenuActions.setDefaultItems()
                                }
                                itemMenuActions.setCurrent(data)
                                itemMenuActions.setCoords(evt.clientX, evt.clientY)
                                evt.stopPropagation()
                            }
                        } />)}
                </FormLoader>

            </div>
            <Menu {...menu} />
            <Menu {...itemMenu} />
        </>
    )
}

export default FormFileBranch
