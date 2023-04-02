import css from './form-button.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';


const FormButton = ({ text, icon, style = 2, loading, disabled = false, onClick = () => null, ...props }) => {
    return (
        <div {...props} className={[css.block, css[`block-style--${style}`]].join(' ')} onClick={
            () => { if (!disabled) onClick() }
        }>
            <div className={css.icon}>
                <FontAwesomeIcon icon={icon} />
            </div>
            <span className={css.text}>{text}</span>
        </div>
    );
}

export default FormButton;
