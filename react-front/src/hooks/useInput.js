
import useValidateInput from './useValidateInput'


function useInput({ value, validation_methods }) {
    const { onChanged } = useValidateInput({value, ...validation_methods})

    const getProps = () => ({
        value,
        onChange: (evt) => {
            return onChanged(evt.target.value)
        }
    })


    return { getProps }
}

export default useInput
