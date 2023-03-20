import css from './form-filebranch.module.css'
import FileBranchItem from './filebranch-item'
import { useMenu } from 'hooks/useMenu'
import Menu from 'components/UI/Menu'
import FormLoader from '../form-loader'
import FilesService from 'services/filesService'


function FormFileBranch({ items, actions, state, loading }) {


    const RemoveItem = async (id) =>{
        
        await FilesService.remove(id).then(
            ()=>{
                actions.removeItem(id)
            }
        )
    }

    const [menu, menuActions] = useMenu([
        { text: 'Создать', icon: 'fa-solid fa-trash', action: () => '' }
    ])
    const [itemMenu, itemMenuActions] = useMenu([
        { text: 'Переименовать', icon: 'fa-solid fa-pen', action: (current) => state.editModeOn(current.guid), RemoveItem},
        { text: 'Удалить', icon: 'fa-solid fa-trash', action: (current) => RemoveItem(current.guid)},
    ])




    return (
        <>
            <div className={css.block} onContextMenu={(evt) => {
                evt.preventDefault()
                menuActions.open()

                menuActions.setCoords(evt.clientX, evt.clientY)
            }}>
                <FormLoader loading={loading}>
                    {items.map((item) => <FileBranchItem
                        key={'FBI' + item.value.guid}

                        {...item}
                        actions={actions.getItem(item.value.guid)}
                        onMenu={
                            (evt, data, editMode) => {
                                evt.preventDefault()
                                itemMenuActions.open()
                                if (editMode) {
                                    itemMenuActions.changeItems([{ text: 'Сохранить', icon: "fa-solid fa-check", action: (current) => state.editModeOff(current.guid) }])
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
