import useInput from 'hooks/useInput'
import Input from 'components/UI/Input'
import { useState, useEffect } from 'react';


function FormInput({ value, hidden = false, validation_methods, ...props }) {

    const [localHidden, setLocalHidden] = useState(hidden)

    useEffect(() => {
        setLocalHidden(hidden)
    }, [hidden])


    const { getProps } = useInput({ value, validation_methods })

    return !localHidden ? <Input {...props} {...getProps()} /> : null

}

export default FormInput
