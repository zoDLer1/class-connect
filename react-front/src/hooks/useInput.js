
import useValidateInput from './useValidateInput'


function useInput({ validate, value, onChange, rools }) {
    
    const { onChanged } = useValidateInput(validate, value, onChange)

    const getProps = () => ({
        value,
        onChange: (evt) => {
            for (const rool of rools){
                if (!rool(evt.target.value)){
                    return
                }
            }
            return onChanged(evt.target.value)
        }
    })


    return { getProps }
}

export default useInput
