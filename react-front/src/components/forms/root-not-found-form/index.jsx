import css from './root-not-found-form.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faRotateRight } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';


const RootNotFoundForm = () => {
    const navigate = useNavigate()
    return (
        <div className={css.block}>
            <h2 className={css.title}>Вы еще не добавленны ни в один класс</h2>
            <div className={css.icon} onClick={()=>navigate('/files')} >
                <FontAwesomeIcon icon={faRotateRight} size='2xl' />
            </div>

        </div>
    );
}

export default RootNotFoundForm;
