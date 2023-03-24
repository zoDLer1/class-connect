import useInput from 'hooks/useInput'
import Input from 'components/UI/Input'


function FormInput({ validate, value, onChange, hidden=false, ...props}) {

    const { getProps } = useInput({validate, value, onChange})
    
    return !hidden ? <Input {...props} {...getProps()} /> : null
    
}

export default FormInput
