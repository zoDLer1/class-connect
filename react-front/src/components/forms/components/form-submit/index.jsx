import css from './form-submit.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faSpinner } from '@fortawesome/free-solid-svg-icons'

function FormSubmit({ text, onClick, loading = false, ...props }) {

    return (
        <div className={css.block}>
            <button {...props} onClick={(evt)=>{evt.preventDefault(); onClick()}} className={css.title}>{loading ? <FontAwesomeIcon icon={faSpinner} size="lg" spinPulse /> : text}</button>
        </div>
    )
}

export default FormSubmit
