
import useValidateInput from './useValidateInput'


function useInput({ validate, value, onChange }) {

    const { onChanged } = useValidateInput(validate, value, onChange)

    const getProps = () => ({
        value,
        onChange: (evt) => onChanged(evt.target.value)
    })


    return { getProps }
}

export default useInput
