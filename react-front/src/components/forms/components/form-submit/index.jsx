import css from './form-submit.module.css'


function FormSubmit({ text, ...props }) {

    return (
        <div className={css.block}>
            <div {...props} className={css.title}>{text}</div>
        </div>
    )
}

export default FormSubmit
