
import useValidateInput from './useValidateInput'


function useDateTime({ value, validation_methods }) {

    const { onChanged } = useValidateInput({ value, ...validation_methods })

    const getTime = () => {

    }
    const getProps = () => ({
        value,
        onChange: (evt) => {
            console.log(evt.target.value)
            return onChanged(evt.target.value)
        }
    })


    return { getProps }
}

export default useDateTime
