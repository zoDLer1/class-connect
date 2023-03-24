import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSpinner } from '@fortawesome/free-solid-svg-icons'
import css from './form-loader.module.css'


const FormLoader = ({ loading=true, children }) => {
    return loading
        ? <div className={css.block}>
            <FontAwesomeIcon icon={faSpinner} size="2xl" spinPulse />
        </div>
        : children
}

export default FormLoader;
