import React from 'react';
import FormInfoElem from '../formFileElem';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faXmark } from '@fortawesome/free-solid-svg-icons';

const FormInfoUser = ({ action, ...props }) => {
    return (

        <FormInfoElem {...props}>
            <FontAwesomeIcon icon={faXmark} size='lg' onClick={() => action(props.id)} color='#DB4343' />
        </FormInfoElem>

    );
}

export default FormInfoUser;
