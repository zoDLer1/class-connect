import css from './filebranch-item.module.css'
import _uniqueId from 'lodash/uniqueId';
import { useState } from 'react';


function FileBranchItem({ data }) {

    const [id] = useState(_uniqueId('FBI-'))

    return (
        <div className={css.block}>
            <input type="radio" hidden id={id} name='filebrach-item' />
            <label htmlFor={id} className={css.body}>
                <i className={`${css.icon} fa-solid fa-folder`}></i>
                <p className={css.title}>{data.name}</p>
            </label>
            <i className={`${css.arrow} fa-solid fa-angle-right`}></i>
            
        </div>
    )
}

export default FileBranchItem
