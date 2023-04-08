import React from 'react';
import FormInfoElem from '../formFileElem';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';
import css from './formInfoUser.module.css'


const FormInfoUser = ({ action, ...props }) => {
    return (

        <FormInfoElem {...props} className={css.block}>
            <div className={css.icon}>
                <FontAwesomeIcon icon={faXmark} size='lg' onClick={() => action(props.id)}  />
            </div>

        </FormInfoElem>

    );
}

export default FormInfoUser;
