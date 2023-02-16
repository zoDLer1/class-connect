import css from './form-checkbox.module.css'


function FormCheckbox({ id, text, ...props }) {
    return (
        <div className={css.block}>
            <input {...props} hidden className={css.checkbox} type="checkbox" id={id} />
            <label htmlFor={id} className={css.title}>{text}</label>
        </div>
    )
}

export default FormCheckbox
