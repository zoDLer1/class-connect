import { useState } from "react";
import { ClosingContext } from 'contexts/closingContext'
import { useContext } from 'react'
import _uniqueId from 'lodash/uniqueId';


function useOpen(autoClose=true) {
    
    const [isOpen, setOpen] = useState(false)
    const { add, remove } = useContext(ClosingContext)
    const [id] = useState(_uniqueId())
    
    const open = () => {
        setOpen(true)
        add({id, close})
    }
    const close = () => {
        setOpen(false)
        remove(id)
    }
    const toggle = () =>{
        isOpen ? close() : open()
    }

    return {condition:isOpen, open, close, toggle} 
}

export default useOpen
