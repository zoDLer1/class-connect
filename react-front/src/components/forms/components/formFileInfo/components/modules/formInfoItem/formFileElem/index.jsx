import css from './formFileElem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FormInfoElem = ({ id, title, value, icon, children, className, ...props }) => {
    return (
        <div {...props} className={[css.block, className].join(' ')}> 
            <div className={css.body}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='lg' />
                </div>
                <h3 className={[css.title, !value && css['title-extended']].join(' ')}>{title}</h3>
                {
                    value && <div className={css.value}>{value}</div>
                }
            </div>
            <div className={css.actions}>
                {children}
            </div>
        </div>
    );
}

export default FormInfoElem;
