import css from './form-select.module.css'
import useInput from 'hooks/useInput';

const FormSelect = ({ title, error, options, rools=[], validate, onChange = () => null, value, hidden = false }) => {

    const { getProps } = useInput({ validate, value, onChange, rools })

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
