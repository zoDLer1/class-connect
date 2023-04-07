
import useValidateInput from './useValidateInput'


function useSelect({ value,  }) {

    const { onChanged } = useValidateInput({validate, rools, value, onChange})

    const getProps = () => ({
        value,
        onChange: (evt) => {
            setSelected(true)
            onChanged(evt.target.value)
        }
    })


    return { getProps }
}

export default useSelect
