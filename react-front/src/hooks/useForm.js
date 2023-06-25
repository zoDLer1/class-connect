import { useState } from 'react'
import { useRequest } from 'hooks/useRequest'

function useForm(InputsData, request = async () => null, statuses = {}) {


    const DEFAULT_STATUSES = {
        400: (response) => handleServerErrors(response.response.data.errors)
    }


    const [send, waitingForResponse] = useRequest(request, { ...DEFAULT_STATUSES, ...statuses })
    const [inputs, setInputs] = useState(InputsData)
    const [errors, setErrors] = useState({})

    const addInput = (name, input_data) => {
        const newInputsData = { ...InputsData }
        newInputsData[name] = input_data
        setInputs(newInputsData)
    }

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

    const checkRools = (inputName, newValue) => {

        const rools = inputs[inputName]?.rools || []

        for (const rool of rools){
            if (!rool(newValue)){
                return false
            }
        }
        return true
    }
    const getInput = (inputName) => {
        return {
            value: inputs[inputName].value,
            error: errors[inputName],
            disabled: waitingForResponse,
            hidden: inputs[inputName].hidden,
            options: inputs[inputName].options,
            validation_methods: {
                rools: (newValue) => checkRools(inputName, newValue),
                validate: () => validateInput(inputName),
                setValue: (value) => setInputValue(inputName, value)
            }

        }
    }
    const validateInput = (inputName) => {

        const validators = inputs[inputName]?.validators || []

        for (const validator of validators) {
            if (!inputs[inputName].hidden) {
                const errorMessage = validator(inputs[inputName].value)
                changeError(inputName, errorMessage)
                if (errorMessage) {
                    return true
                }
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
                if (!inputs[key].hidden) {
                    validatedData[key] = inputs[key].value
                }
            }
        }
        return validatedData

    }
    const handleServerErrors = (errors) => {
        for (const key in errors) {
            changeError(key, errors[key][0])
        }
    }
    const getSubmit = () => {
        return {
            onClick: onSubmit,
            loading: waitingForResponse
        }
    }
    const InputHide = (inputName) => {
        setInputs((newInputsData) => {
            newInputsData[inputName].hidden = true
            return newInputsData
        })
    }
    const InputShow = (inputName, newValue) => {
        setInputs((newInputsData) => {

            newInputsData[inputName].hidden = false
            if (newValue) {
                newInputsData[inputName].value = newValue
            }
            return newInputsData
        })
    }
    const onSubmit = async () => {
        const validated_data = getValidatedData()
        if (Object.keys(validated_data).length) {
            await send(validated_data)
        }

    }


    return { InputsData:inputs, setInputValue, InputHide, InputShow, getSubmit, handleServerErrors, getInput, getValidatedData, addInput }

}

export default useForm
