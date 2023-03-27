import css from '../formFileInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormFileInfoElem from './formFileInfoElem';

const FormFileInfoItem = ({ icon, title, items }) => {
    return (
        <div className={css.block}>
            <div className={css.header}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='xl'/>
                </div>
                <h3 className={css.title}>{title}</h3>
            </div>
            <div className={css.items}>
                {items.map((item, index) => <FormFileInfoElem key={index} {...item}/>)}
            </div>
        </div>
    );
}

export default FormFileInfoItem;
