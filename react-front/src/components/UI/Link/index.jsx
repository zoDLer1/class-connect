import BaseLink from "../BaseLink"
import css from './link.module.css'



function Link({size=2, ...props}) {
    return (
        <BaseLink className={[css.block, css[`size-${size}`]].join(' ')} {...props} />
    )
}

export default Link
