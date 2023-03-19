import { useEffect, useContext } from "react"
import { CloseContext } from 'contexts/сloseContext'



export const useMenuCloseing = (itemMenu, itemMenuActions) => {
    const { closeAll } = useContext(CloseContext)


    useEffect(()=>{
        if (!itemMenu.condition && itemMenu.coords[0] && itemMenu.coords[1]){
            closeAll()
            itemMenuActions.open() 
        }
        // !!!
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [itemMenu.coords]) 
    
}
