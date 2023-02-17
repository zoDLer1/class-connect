import { useState } from "react";



function useOpen() {
    const [isOpen, setOpen] = useState(false)
    
    const open = () => {
        setOpen(true)
    }
    const close = () => {
        setOpen(false)
    }
    const toggle = () =>{
        isOpen ? close() : open()
    }
    return {condition:isOpen, open, close, toggle} 
}

export default useOpen
