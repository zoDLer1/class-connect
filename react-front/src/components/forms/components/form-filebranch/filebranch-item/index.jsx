import css from './filebranch-item.module.css'
import _uniqueId from 'lodash/uniqueId';
import { useState } from 'react';
import FormInput from '../../form-input';

function FileBranchItem({ data, onMenu, editMode }) {

    const [id] = useState(_uniqueId('FBI-'))
   
    return (
        <div className={css.block} onContextMenu={(evt) => onMenu(evt, data, editMode)}>
            <input type="radio" hidden id={id} name='filebrach-item' />
            
            <label htmlFor={id} className={css.body}>
                <i className={`${css.icon} fa-solid fa-folder`}></i>
                {editMode 
                ? <FormInput defaultValue={data.name}/>
                : <p className={css.title}>{data.name}</p>
                }
            </label>
            <i className={`${css.arrow} fa-solid fa-angle-right`}></i>
            
        </div>
    )
}

export default FileBranchItem
