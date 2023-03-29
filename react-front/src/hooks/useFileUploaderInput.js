
import useValidateInput from './useValidateInput'


function useFileUploaderInput({ validate, value, onChange }) {

    const { onChanged } = useValidateInput(validate, value, onChange)

    const Remove = (indx) =>{
        onChanged(value.filter((item, index) => index !== indx))
    }

    const getProps = () => ({
        onChange: (evt) => onChanged([...value, ...Array.from(evt.target.files)])
    })


    return { getProps, Remove }
}

export default useFileUploaderInput
