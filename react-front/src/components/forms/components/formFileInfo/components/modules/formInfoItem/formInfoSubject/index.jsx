import React from 'react';
import FormInfoElem from '../formFileElem';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import css from './formInfoSubject.module.css'

const FormInfoSubject = (props) => {

    const navigate = useNavigate()
    return (

        <FormInfoElem className={css.block} onClick={() => navigate('/files/' + props.id)} {...props}>
            <div className={css.icon}>
                <FontAwesomeIcon icon={faAngleRight} size='lg' />
            </div>
        </FormInfoElem>



    );
}


export default FormInfoSubject;

