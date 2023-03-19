import { useOpen } from "./useOpen"
import { useState } from "react"

export function usePopup(onAutoClose = () => null) {

    const [current, setCurrent] = useState()

    const onPopupAutoClose = () =>{
        setCurrent((current)=> {
            onAutoClose(current)
            return current
        })
        
    }

    const openHook = useOpen(onPopupAutoClose)
    


    return {...openHook, current, setCurrent}

}


