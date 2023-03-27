import css from './formFileInfoElem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FormFileInfoElem = ({ title, value, icon }) => {
    return (
        <div className={css.block}>
            <div className={css.icon}>
                <FontAwesomeIcon icon={icon} size='lg' /> 
            </div>
            <h3 className={css.title}>{title}</h3>
            <p className={css.value}>{value}</p>
        </div>
    );
}

export default FormFileInfoElem;
