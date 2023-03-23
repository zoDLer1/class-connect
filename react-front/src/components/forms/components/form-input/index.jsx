import useInput from 'hooks/useInput'
import Input from 'components/UI/Input'


function FormInput({ validate, value, onChange, ...props}) {

    const { getProps } = useInput({validate, value, onChange})
    
    return (
        <Input {...props} {...getProps()} />
    )
}

export default FormInput
