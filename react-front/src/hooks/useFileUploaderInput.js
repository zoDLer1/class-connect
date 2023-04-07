
import useValidateInput from './useValidateInput'


function useFileUploaderInput({ value, validation_methods  }) {
    const { onChanged } = useValidateInput({value, ...validation_methods })

    const Remove = (indx) =>{
        onChanged(value.filter((item, index) => index !== indx))
    }

    const getProps = () => ({
        onChange: (evt) => onChanged([...value, ...Array.from(evt.target.files)])
    })


    return { getProps, Remove }
}

export default useFileUploaderInput
