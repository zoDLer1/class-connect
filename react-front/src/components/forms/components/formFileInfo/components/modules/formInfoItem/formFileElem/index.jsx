import css from './formFileElem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FormInfoElem = ({ id, title, value, icon, action, actionFunc }) => {
    return (
        <div className={css.block}> 
            <div className={css.body}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='lg' />
                </div>
                <h3 className={css.title}>{title}</h3>
                <p className={css.value}>{value}</p>
            </div>
            <div className={css.actions}>
                <div onClick={()=>actionFunc(id)} className={css.action}>
                    {action}
                </div>
                
            </div>

        </div>
    );
}

export default FormInfoElem;
