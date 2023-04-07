import css from './form-select.module.css'
import useInput from 'hooks/useInput';

const FormSelect = ({ title, error, options, value, hidden = false, validation_methods }) => {

    const { getProps } = useInput({ value, validation_methods })

    if (!hidden) {
        return (
            <div className={css.block}>
                {title && <h4 className={css.title}>{title}</h4>}
                <div className={css.body}>
                    <select className={css.input} {...getProps()}>
                        {options.map(item => <option value={item.id} key={item.id}>{item.text}</option>)}
                    </select>
                    <span className={css.error}>{error}</span>
                </div>
            </div>

        );
    }
}

export default FormSelect;
