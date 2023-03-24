import { useEffect, useState } from 'react'

function useValidateInput(validate, value, onInputChange) {

    const [isSelected, setSelected] = useState(false)

    useEffect(() => {
        if (isSelected) {
            validate()
        }

        /* eslint-disable react-hooks/exhaustive-deps */
    }, [value])

    const onChanged = (evt) => {
        setSelected(true)
        onInputChange(evt)
    }


    return { onChanged }
}

export default useValidateInput
