import css from './form-filebranch.module.css'
import FileBranchItem from './filebranch-item'
import { useMenu } from 'hooks/useMenu'
import Menu from 'components/UI/Menu'


function FormFileBranch({ items, actions, state }) {





    const [menu, menuActions] = useMenu([
        { text: 'Создать', icon: 'fa-solid fa-trash', action: () => '' }
    ])
    const [itemMenu, itemMenuActions] = useMenu([
        { text: 'Переименовать', icon: 'fa-solid fa-pen', action: (current) => state.editModeOn(current.guid) },
        { text: 'Удалить', icon: 'fa-solid fa-trash', action: (current) => console.log(current) },
    ])




    // const EditModeOn = (guid) => {
    //     // const newItems = [...items]
    //     // for (let i=0; i < newItems.length; i++){
    //     //     newItems[i].editMode = false
    //     // }
    //     // newItems[items.findIndex(item => item.data.guid === guid)].editMode = true
    //     // setItems(newItems)
    // }    
    // const EditModeOff = (guid) => {
    //     // const newItems = [...items]
    //     // newItems[items.findIndex(item => item.data.guid === guid)].editMode = false
    //     // setItems(newItems)
    // }
    // const memoizedItems = useMemo(()=>{
    //     return data
    // }, [data])



    return (
        <>
            <div className={css.block} onContextMenu={(evt) => {
                evt.preventDefault()
                menuActions.open()
                
                menuActions.setCoords(evt.clientX, evt.clientY)
            }}>
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
            </div>
            <Menu {...menu} />
            <Menu {...itemMenu} />
        </>
    )
}

export default FormFileBranch
