import css from './filebranch-item.module.css'
import _uniqueId from 'lodash/uniqueId';
import { useState } from 'react';
import FormInput from '../../form-input';

function FileBranchItem({ value, onMenu, state, actions }) {

    const [id] = useState(_uniqueId('FBI-'))

    return (
        
        <div className={css.block} onContextMenu={(evt) => {
            onMenu(evt, value, state.editMode)
            
            
            // if(state.editMode){
            //     evt.stopPropagation()
            // }
        }}>
            <input type="radio" hidden id={id} name='filebrach-item' />

            <label htmlFor={id} className={css.body}>
                <i className={`${css.icon} fa-solid fa-folder`}></i>
                {state.editMode
                    ? <FormInput style={{ cursor: 'text' }} onClick={(evt) => evt.stopPropagation()} onChange={(evt)=>actions.setProp('name', evt.target.value)} value={value.name} />
                    : <p className={css.title}>{value.name}</p>
                }
            </label>
            <i className={`${css.arrow} fa-solid fa-angle-right`}></i>

        </div>
    )
}

export default FileBranchItem
