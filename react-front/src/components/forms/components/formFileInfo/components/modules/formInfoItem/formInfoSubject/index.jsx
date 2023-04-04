import React from 'react';
import FormInfoElem from '../formFileElem';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleRight } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';


const FormInfoSubject = (props) => {
    
    const navigate = useNavigate()
    return (
        <div style={{cursor: 'pointer'}} onClick={() => navigate('/files/' + props.id)}>
            <FormInfoElem {...props}>
                <FontAwesomeIcon icon={faAngleRight} size='lg' color='var(--dark-op25-color)' />
            </FormInfoElem>
        </div>


    );
}

export default FormInfoSubject;
