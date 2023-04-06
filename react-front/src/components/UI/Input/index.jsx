import css from './input.module.css'


function Input({ title, type='text', error, ...props }) {
    return (
        <div className={css.block}>
            {title && <h4 className={css.title}>{title}</h4>}
            <div className={css.body}>
                <input type={type} className={css.input} {...props} />
                <span className={css.error}>{error}</span>
            </div>
        </div>
    )
}

export default Input;
