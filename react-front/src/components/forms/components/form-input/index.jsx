import css from './form-input.module.css'
import useInput from 'hooks/useInput'

function FormInput({title, error, validate, value, onChange, ...props}) {

    const { getProps } = useInput({validate, value, onChange})

    return (
        <div className={css.block}>
            {title && <h4 className={css.title}>{title}</h4>}
            <div className={css.body}>
                <input className={css.input} {...props} {...getProps()} />
                <span className={css.error}>{error}</span>
            </div>
        </div>
    )
}

export default FormInput
