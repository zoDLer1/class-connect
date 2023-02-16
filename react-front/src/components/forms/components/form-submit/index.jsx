import css from './form-submit.module.css'


function FormSubmit({ id, text, ...props }) {
    return (
        <div className={css.block}>
            <input {...props} hidden className={css.submit} type="submit" id={id} />
            <label for={id} className={css.title}>{text}</label>
        </div>
    )
}

export default FormSubmit
