import { useState } from "react"
import { usePopup } from "./usePopup"


export function useMenu(itms, onAutoClose=()=> null) {
    const {condition, current, setCurrent, ...popupHook} = usePopup(onAutoClose)
    const [items, changeItems] = useState(itms) 
    const [coords, set] = useState([0, 0])
    const setCoords = (x, y) => set([x,y])
    const setDefaultItems = () => changeItems(itms)
    
    return [{condition, items, coords, current}, {setCoords, setDefaultItems, changeItems, setCurrent, ...popupHook}]
}

