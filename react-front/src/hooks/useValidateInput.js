import { useEffect, useState } from 'react'

function useValidateInput({value, validate, rools, setValue}) {

    const [isSelected, setSelected] = useState(false)

    useEffect(() => {
        if (isSelected) {
            validate()
        }

        /* eslint-disable react-hooks/exhaustive-deps */
    }, [value])

    const onChanged = (value) => {
        setSelected(true)
        const isValid = rools(value)
        if (isValid){
            setValue(value)
        }
    }


    return { onChanged }
}

export default useValidateInput
