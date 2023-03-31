import css from './formFileInfoTaskFile.module.css'
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FormFileInfoTaskFile = ({ name, remove, isSubmitted }) => {
    return (
        <div className={css.block}>
            <h4 className={css.title}>{name}</h4>
            {!isSubmitted &&
                <div onClick={remove} className={css.icon}>
                    <FontAwesomeIcon icon={faXmark} size='lg' />
                </div>
            }           
        </div>
    );
}

export default FormFileInfoTaskFile;
