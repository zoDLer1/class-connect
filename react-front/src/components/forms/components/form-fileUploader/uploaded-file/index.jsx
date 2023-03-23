import css from './uploaded-file.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';


const UploadedFile = ({ text }) => {
    return (
        <div className={css.block}>
            <span className={css.text}>{text}</span>
            <FontAwesomeIcon icon={faXmark}/>
        </div>
    );
}

export default UploadedFile;
