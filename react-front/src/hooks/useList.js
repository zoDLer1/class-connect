import { useState } from "react";
import { CloseContext } from 'contexts/сloseContext';
import { useContext } from "react";




export const useList = (onAutoClose = () => null) => {



    const [list, setList] = useState([])



    const setItems = (items) => {
        setList(() => {
            let newList = []
            for (const item of items) {
                newList.push({ value: item, stored: {}, state: { loading: false, editMode: false } })
            }
            return newList
        })

    }

    const { add, remove } = useContext(CloseContext)

    const findItemIndex = (lst, id) => {
        return [...lst].findIndex(item => item.value.guid === id)
    }
    const storeProp = (id, key) => {
        setList(
            (list) => {
                const newList = [...list]
                const index = findItemIndex(list, id)
                newList[index].stored[key] = newList[index].value[key]
                return newList
            }
        )

    }
    const reject = (id, key) => {
        setList((list) => {
            const newList = [...list]
            const index = findItemIndex(list, id)
            newList[index].value[key] = newList[index].stored[key]
            delete newList[index].stored[key]
            return newList
        })




    }
    const commit = (id, key) => {
        setList((list) => {
            const newList = [...list]
            const index = findItemIndex(list, id)
            delete newList[index].stored[key]
            return newList
        })
    }
    const loadingState = (id, value) => {
        setList((list) => {
            const newList = [...list]
            newList[findItemIndex(list, id)].state.loading = value
            return newList
        })

    }
    const editModeState = (id, value) => {
        setList((list) => {
            const newList = [...list]
            newList[findItemIndex(list, id)].state.editMode = value
            return newList
        })
    }
    const editModeOn = (id) => {

        editModeState(id, true)

        add({
            id, close: () => {
                editModeOff(id)
                onAutoClose(list[findItemIndex(list, id)])
            }
        })
    }
    const editModeOff = (id) => {
        editModeState(id, false)
        remove(id)

    }
    const setItemProp = (id, key, value) => {
        setList((list) => {
            const newList = [...list]
            newList[findItemIndex(list, id)].value[key] = value
            return newList
        })

    }
    const addItem = (data) => {
        setList((list) => [...list, { value: data, stored: {}, state: { loading: false, editMode: false } }])
    }
    const updateItem = (id, data) => {
        setList((list) => {
            const newList = [...list]
            newList[findItemIndex(list, id)].value = data
            return newList
        })
    }
    const getItem = (id) => {
        return {
            update: (data) => updateItem(id, data),
            remove: () => removeItem(id),
            setProp: (key, value) => setItemProp(id, key, value),
            editModeOn: () => editModeOn(id),
            editModeOff: () => editModeOff(id),
            loading: (value) => loadingState(id, value)
        }

    }
    const removeItem = (id) => {
        setList((list) => [...list].filter(item => item.value.guid !== id))
    }
    return [list, { setItems, addItem, updateItem, removeItem, setItemProp, getItem }, { editModeOn, editModeOff, loadingState }, { storeProp, reject, commit }]
}