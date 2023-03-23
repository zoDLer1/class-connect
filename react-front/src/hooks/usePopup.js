import { useState } from "react"
import { useCurrent } from "./useCurrent"

export function usePopup(onAutoClose = () => null) {


    const currentHook = useCurrent(onAutoClose)
    const [content, setContent] = useState(<></>)
    


    return {...currentHook, content, setContent}

}


