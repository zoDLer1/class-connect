import css from './uploaded-file.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';


const UploadedFile = ({ text, onClose }) => {
    return (
        <div className={css.block}>
            <span className={css.text}>{text}</span>
            <div className={css.close} onClick={onClose}>
                <FontAwesomeIcon icon={faXmark} />
            </div>

        </div>
    );
}

export default UploadedFile;
