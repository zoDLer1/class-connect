import { useState } from 'react'

function useForm(InputsData) {


    const [inputs, setInputs] = useState(InputsData)
    const [errors, setErrors] = useState({})

    const changeError = (inputName, error) => {
        setErrors((errors) => {
            const newErrors = { ...errors }
            if (error) {
                newErrors[inputName] = error
            }
            else {
                delete newErrors[inputName]
            }
            return newErrors
        })
    }
    const setInput = (inputName, key, value) => {
        const newInputs = { ...inputs }
        newInputs[inputName][key] = value
        setInputs(newInputs)
    }
    const setInputValue = (inputName, value) => setInput(inputName, 'value', value)
    const getInput = (inputName) => {
        return {
            onChange: (evt) => setInputValue(inputName, evt.target.value),
            value: inputs[inputName].value,
            validate: () => validateInput(inputName),
            error: errors[inputName]

        }
    }
    const validateInput = (inputName) => {

        for (const validator of inputs[inputName].validators) {
            const errorMessage = validator(inputs[inputName].value)
            changeError(inputName, errorMessage)
            if (errorMessage) {
                return true
            }
        }
        return false

    }
    const validateInputs = () => {
        let error = false
        for (const key in inputs) {
            const inputError = validateInput(key)
            if (!error) {
                error = inputError
            }
        }
        return error
    }
    const getValidatedData = () => {
        const validatedData = {}
        const error = validateInputs()
        if (!error) {
            for (const key in inputs) {
                validatedData[key] = inputs[key].value
            }
        }
        return validatedData

    }



    return { changeError, getInput, getValidatedData }

}

export default useForm
