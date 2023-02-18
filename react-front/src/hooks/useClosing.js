import { useState } from "react";




function useClosing() {
    const [items, set] = useState([]) 
    

    const add = (item) => {
        set((items) => [...items, item])   
    }
    
    const remove = (id) =>{
        set((items) => [...[...items].filter(item => item.id !== id)])
    }

    const closeAll = () =>{
        for (const item of items){
                item.close()
        }
    }

    return { itms: items, add, remove, closeAll}
}

export default useClosing