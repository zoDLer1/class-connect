import { useState } from "react";
import { CloseContext } from 'contexts/сloseContext';
import { useContext } from "react";





export const useList = (onAutoClose = () => null) => {

    const [collection, setList] = useState({})

    const [selectedItem, setSelectedItem] = useState(null)

    const [edited, setEdited] = useState(false)

    const { add, remove } = useContext(CloseContext)


    // * Items Collection Methods
    const setItems = (items) => {
        setList(() => {
            let newCollection = {}
            for (const item of items) {
                newCollection[item.guid] = { value: item, stored: {}, state: { loading: false, editMode: false, selected: false } }
            }
            return newCollection
        })
    }

    // * Item Store Methods




    const storeProp = (id, key) => {
        setList(
            (collection) => {
                const newCollection = {...collection}
                newCollection[id].stored[key] = newCollection[id].value[key]
                return newCollection
            }
        )
    }
    const reject = (id, key) => {
        setList((collection) => {
            const newCollection = {...collection}
            newCollection[id].value[key] = newCollection[id].stored[key]
            delete newCollection[id].stored[key]
            return newCollection
        })
    }
    const commit = (id, key) => {
        setList((collection) => {
            const newCollection = {...collection}
            delete newCollection[id].stored[key]
            return newCollection
        })
    }

    // * Item State Methods










    const selectedStateOn = (id) => {
        if (!edited){
            setList((collection) => {
                let newCollection = {...collection}
                newCollection = unselectAll(newCollection)
                newCollection[id].state.selected = true
                setSelectedItem(newCollection[id].value)
                return newCollection
            })

            if (!selectedItem) {
                add({
                    id: 'selected-items',
                    close: () => {
                        setSelectedItem(null)
                        selectedStateOff()
                        console.log('close')
                    }
                })
            }
        }

    }
    const unselectAll = (newCollection) => {
        for (const id in newCollection) {
            newCollection[id].state.selected = false
        }
        return newCollection
    }
    const selectedStateOff = () => {
        setList((collection) => {
            return unselectAll({...collection})
        })
        remove('selected-items')
    }


    const editModeState = (id, value) => {
        setList((collection) => {
            const newCollection = {...collection}
            newCollection[id].state.editMode = value
            return newCollection
        })
    }

    // ? refactor it
    const editModeOn = (id) => {

        editModeState(id, true)
        setEdited(true)
        add({
            id, close: () => {
                editModeOff(id)
                onAutoClose(collection[id])
            }
        })
    }
    const editModeOff = (id) => {
        setEdited(false)
        editModeState(id, false)
        remove(id)

    }


    const setItemProp = (id, key, value) => {
        setList((collection) => {
            const newCollection = {...collection}
            newCollection[id].value[key] = value
            return newCollection
        })

    }

    const appendItem = (data) => {
        setList((collection) => {
            const newCollection = {...collection}
            newCollection[data.inewCollectiond] = { value: data, stored: {}, state: { loading: false, editMode: false, selected: false } }
            return
        })
    }

    const updateItem = (id, data) => {
        setList((collection) => {
            const newCollection = {...collection}
            newCollection[id].value = data
            return newCollection
        })
    }
    const getItem = (id) => {
        return {
            update: (data) => updateItem(id, data),
            remove: () => removeItem(id),
            setProp: (key, value) => setItemProp(id, key, value),
            editModeOn: () => editModeOn(id),
            editModeOff: () => editModeOff(id),
            select: () => selectedStateOn(id)
        }
    }
    const removeItem = (id) => {
        setList((collection) => {
            const newCollection = {...collection}
            delete newCollection[id]
            return newCollection
        })
    }
    return [collection, { setItems, addItem: appendItem, updateItem, removeItem, setItemProp, getItem }, { editModeOn, selectedStateOn, editModeOff }, { storeProp, reject, commit }, selectedItem]
}
