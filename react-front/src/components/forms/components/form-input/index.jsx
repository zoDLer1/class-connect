import useInput from 'hooks/useInput'
import Input from 'components/UI/Input'


function FormInput({ value, hidden=false, validation_methods, ...props}) {


    const { getProps } = useInput({value, validation_methods})

    return !hidden ? <Input {...props} {...getProps()} /> : null

}

export default FormInput
