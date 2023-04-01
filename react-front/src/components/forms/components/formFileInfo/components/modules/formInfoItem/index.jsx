import css from '../formInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormInfoElem from './formFileElem';

const FormInfoItem = ({ icon, title, items, Elem = FormInfoElem }) => {
    return (
        <div className={css.block}>
            <div className={css.header}>
                <div className={css.header_body}>
                    <FontAwesomeIcon icon={icon} size='xl' color='var(--primary-color)' />
                    <h3 className={css.title}>{title}</h3>
                </div>
            </div>
            <div className={css.items}>
                {items.map((item, index) => <Elem key={index} {...item} />)}
            </div>
        </div>
    );
}

export default FormInfoItem;
