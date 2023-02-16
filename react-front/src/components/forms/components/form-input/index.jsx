import css from './form-input.module.css'


function FormInput({title, ...props}) {
    return (
        <div className={css.block}>
            <h4 className={css.title}>{title}</h4>
            <input className={css.input} {...props} />
        </div>
    )
}

export default FormInput
