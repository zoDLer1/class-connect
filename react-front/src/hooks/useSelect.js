import { useEffect, useState } from 'react'

function useInput({ validate, value, onChange }) {
    // const [errorMessage, setError] = useState('')
    const [isSelected, setSelected] = useState(false)

    useEffect(() => {
        if (isSelected) {
            validate()
        }

        /* eslint-disable react-hooks/exhaustive-deps */
    }, [value])

    const getProps = () => ({
        value,
        onChange: (evt) => {
            setSelected(true)
            onChange(evt.target.value)
        }
    })


    return { getProps }
}

export default useInput
