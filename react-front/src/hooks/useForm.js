import { useState } from 'react'
import { useRequest } from 'hooks/useRequest'

function useForm(InputsData, request, statuses={} ) {


    const DEFAULT_STATUSES = {
        400: (response) => handleServerErrors(response.response.data.errors)
    }
    
    const { send, waitingForResponse } = useRequest(request)
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
            error: errors[inputName],
            disabled: waitingForResponse

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
    const handleServerErrors = (errors) =>{
        for (const key in errors){
            changeError(key, errors[key][0])
        }
    }
    const getSubmit = () =>{
        return {
            onClick: onSubmit,
            loading: waitingForResponse
        }
    }

    const onSubmit = async () =>{
        const validated_data = getValidatedData()
        if(Object.keys(validated_data).length){
            await send(validated_data, {...DEFAULT_STATUSES, ...statuses})
        }
        
    }


    return { getSubmit, handleServerErrors, getInput, getValidatedData }

}

export default useForm
