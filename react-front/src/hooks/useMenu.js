import { useState } from "react"
import useOpen from "./useOpen"


function useMenu(itms) {
    const {condition, ...openHook} = useOpen()
    const [items, changeItems] = useState(itms) 
    const [coords, set] = useState([0, 0])
    const [current, setCurrent] = useState()
    const setCoords = (x, y) => set([x,y])
    const setDefaultItems = () => changeItems(itms)
    
    return [{condition, items, coords, current}, {setCoords, setDefaultItems, changeItems, setCurrent, ...openHook}]
}

export default useMenu
