import css from './alert.module.css'


export default ({ hook }) =>  {


    const AnimEnd = (evt) => {
        if (evt.animationName.startsWith('alert_opening'))
        hook.hide()
    }

    if (hook.isShowing){
        return (
            <div onAnimationEnd={AnimEnd} className={css.block}>
                {hook.info}
            </div>
        )
    }
    
}
