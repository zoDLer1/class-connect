import React from 'react';
import pagesCss from '../pages.module.css'
import RootNotFoundForm from 'components/forms/root-not-found-form';


const RootNotFound = () => {
    return (
        <div className={[pagesCss.default_background, pagesCss.content_position_center].join(' ')}>
            <RootNotFoundForm/>
        </div>
    );
}

export default RootNotFound;
