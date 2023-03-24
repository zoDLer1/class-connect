
import useValidateInput from './useValidateInput'


function useFileUploaderInput({ validate, value, onChange }) {

    const { onChanged } = useValidateInput(validate, value, onChange)

    const Clear = () =>{
        onChanged(null)
    }

    const getProps = () => ({
        onChange: (evt) => onChanged(evt.target.files[0])
    })


    return { getProps, Clear }
}

export default useFileUploaderInput
