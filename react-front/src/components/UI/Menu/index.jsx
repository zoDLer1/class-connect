import css from './menu.module.css'


function Menu({ current, coords, condition, items }) {

    return (
        <>
            {condition &&
                <div  className={css.block} style={{left: coords[0], top: coords[1]}}>
                    <div className={css.items}>
                        {items.map((item, index) => {
                             
                            return  <div onClick={(evt)=>{
                                        item.action(current); 
                                        if (item.noneAutoClose)
                                            evt.stopPropagation()
                                        }} key={index} className={css.item} >
                                        <i className={`${item.icon} ${css.icon}`}></i>
                                        <p className={css.text}>{item.title}</p>
                                    </div>
                            })}
                    </div>
                </div>
            }
        </>
        
    )
}

export default Menu
