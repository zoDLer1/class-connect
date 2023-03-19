import { useState } from "react";



export function useClose() {

    const [items, set] = useState([])


    const add = (item) => {
        set((items) => [...items, item])
    }

    const remove = (id) => {
        set((items) => [...[...items].filter(item => item.id !== id)])
    }

    const closeAll = (excepted) => {
        for (const item of items) {
            item.close()
        }
    }

    return { itms: items, add, remove, closeAll }
}
