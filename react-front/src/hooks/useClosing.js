import { useState } from "react";




function useClosing() {
    const [items, set] = useState([]) 
    

    const add = (item) => {
        set([...items, item])
        
    }
    
    const remove = (id) =>{
        set([...[...items].filter(item => item.id !== id)])
    }

    const closeAll = () =>{
        for (const item of items){
                item.close()
        }
    }

    return { add, remove, closeAll}
}

export default useClosing