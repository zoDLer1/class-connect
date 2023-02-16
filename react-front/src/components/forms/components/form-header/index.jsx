import css from './form-header.module.css'

function FormHeader({ text }) {
    return (
        <div className={css.block}>
            <h3 className={css.title}>{text}</h3>
        </div>
)
}

export default FormHeader
