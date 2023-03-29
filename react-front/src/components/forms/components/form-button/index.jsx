import css from './form-button.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';


const FormButton = ({ text, icon, style=2, loading, ...props }) => {
    return (
        <div {...props} className={[css.block, css[`block-style--${style}`]].join(' ')}>
            <div className={css.icon}>
                <FontAwesomeIcon icon={icon}/>
            </div>
            <span className={css.text}>{text}</span>
        </div>
    );
}

export default FormButton;
