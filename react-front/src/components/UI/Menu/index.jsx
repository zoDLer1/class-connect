import css from './menu.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'




function Menu({ current, coords, condition, items }) {

    return (
        <>

            {condition &&
                <div className={css.block} style={{ left: coords[0], top: coords[1] }}>
                    <div className={css.items}>
                        {items.map((item, index) => {

                            return <div onClick={(evt) => {
                                item.action(current);
                                if (item.noneAutoClose)
                                    evt.stopPropagation()
                            }} key={index} className={css.item}>
                                <div className={css.icon}>
                                    <FontAwesomeIcon icon={item.icon} size='xl' />
                                </div>

                                <p className={css.text}>{item.text}</p>
                            </div>
                        })}
                    </div>
                </div>
            }
        </>

    )
}

export default Menu
