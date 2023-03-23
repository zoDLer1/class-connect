import css from './alert.module.css'


const Alert = ({ isShowing, info, hide }) =>  {


    const AnimEnd = (evt) => {
        if (evt.animationName.startsWith('alert_opening'))
        hide()
    }

    if (isShowing){
        return (
            <div onAnimationEnd={AnimEnd} className={css.block}>
                {info}
            </div>
        )
    }
    
}
export default Alert
