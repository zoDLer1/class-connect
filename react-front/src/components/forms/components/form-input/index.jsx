import useInput from 'hooks/useInput'
import Input from 'components/UI/Input'


function FormInput({ validate=[], rools=[], value, onChange, hidden=false, ...props}) {

    const { getProps } = useInput({validate, value, onChange, rools})
    
    return !hidden ? <Input {...props} {...getProps()} /> : null
    
}

export default FormInput
