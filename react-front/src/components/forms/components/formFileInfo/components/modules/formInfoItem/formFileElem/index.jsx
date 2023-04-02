import css from './formFileElem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FormInfoElem = ({ id, title, value, icon, children }) => {
    return (
        <div className={css.block}> 
            <div className={css.body}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='lg' />
                </div>
                <h3 className={css.title}>{title}</h3>
                <div className={css.value}>{value}</div>
            </div>
            <div className={css.actions}>
                {children}
            </div>
        </div>
    );
}

export default FormInfoElem;
