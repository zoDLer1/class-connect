import css from './popup.module.css'
import React from 'react';



const Popup = ({ condition, content, current, close }) => {
    if (condition) {
        return (
            
            <div className={css.block} onClick={(evt) => {evt.stopPropagation(); close()}}>
                <div onContextMenu={(evt)=>evt.stopPropagation()} onClick={(evt)=>evt.stopPropagation()}>
                    {React.cloneElement(content, {...content.props, current, close})}
                </div>     
            </div>
        )
    }
}

export default Popup;
