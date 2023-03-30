import css from '../formInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormInfoElem from './formFileElem';

const FormInfoItem = ({ icon, title, items }) => {
    return (
        <div className={css.block}>
            <div className={css.header}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='xl'/>
                </div>
                <h3 className={css.title}>{title}</h3>
            </div>
            <div className={css.items}>
                {items.map((item, index) => <FormInfoElem key={index} {...item}/>)}
            </div>
        </div>
    );
}

export default FormInfoItem;
