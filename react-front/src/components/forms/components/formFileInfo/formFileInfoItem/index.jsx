import css from './formFileInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';


const FormFileInfoItem = ({ icon, title, items=[] }) => {
    return (
        <div className={css.block}>
            <div className={css.header}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='xl'/>
                </div>
                <h3 className={css.title}>{title}</h3>
                <div className={css.items}>
                    {/* {items.map((item, index))} */}
                </div>

            </div>
        </div>
    );
}

export default FormFileInfoItem;
