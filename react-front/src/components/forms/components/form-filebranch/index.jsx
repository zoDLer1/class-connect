import css from './form-filebranch.module.css'
import FileBranchItem from './filebranch-item'
import Menu from 'components/UI/Menu'
import useMenu from 'hooks/useMenu'
import { useMemo, useState } from 'react'



function FormFileBranch({ data }) {
    const [items, setItems] = useState([...data].map(
        (item)  => {
            return {
                onMenu:(evt, data, editMode) => {
                    evt.preventDefault() 
                    itemMenuActions.toggle() 

                    editMode 
                    ? itemMenuActions.changeItems([{title: 'Сохранить', icon: "fa-solid fa-check", action: (current) => EditModeOff(current.guid)}])
                    : itemMenuActions.setDefaultItems()

                    itemMenuActions.setCurrent(data)
                    itemMenuActions.setCoords(evt.clientX, evt.clientY) 
                    evt.stopPropagation()
                }, 
                editMode: false,
                data: item,
                key: 'FBI' + item.guid
            }
        }
    ))
    const [menu, menuActions] = useMenu([
        {title: 'Создать', icon: 'fa-solid fa-folder-plus', action: ()=>''},
        {title: 'Создать', icon: 'fa-solid fa-folder-plus', action: ()=>''}
    ])
    const [itemMenu, itemMenuActions] = useMenu([
        {title: 'Переименовать', icon: 'fa-solid fa-pen', action: (current) => EditModeOn(current.guid)},
        {title: 'Удалить', icon: 'fa-solid fa-trash', action: () => ''},
    ])
    const EditModeOn = (guid) => {
        const newItems = [...items]
        for (let i=0; i < newItems.length; i++){
            newItems[i].editMode = false
        }
        newItems[items.findIndex(item => item.data.guid === guid)].editMode = true
        setItems(newItems)
    }




    
    const EditModeOff = (guid) => {
        const newItems = [...items]
        newItems[items.findIndex(item => item.data.guid === guid)].editMode = false
        setItems(newItems)
    }




    const memoizedItems = useMemo(()=>{
        return items
    }, [items])
    

  
    return (
        <>
            <div className={css.block} onContextMenu={(evt) => {evt.preventDefault(); menuActions.toggle(); menuActions.setCoords(evt.clientX, evt.clientY)}}>
                {memoizedItems.map((item)=> <FileBranchItem {...item} />)}
                <Menu {...menu}/>
                <Menu {...itemMenu} />
            </div>
            
        </>
        
    )
}

export default FormFileBranch
