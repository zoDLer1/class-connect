import { useState } from "react"
import { useCurrent } from "./useCurrent"

export function useMenu(onAutoClose=()=> null) {
    const {condition, current, setCurrent, ...popupHook} = useCurrent(onAutoClose)
    const [items, setItems] = useState([]) 
    const [coords, set] = useState([0, 0])
    const setCoords = (x, y) => set([x,y])
 
    
    return [{condition, items, coords, current}, {setCoords, setItems, setCurrent, ...popupHook}]
}

