import css from './filepath-item.module.css'

function FilePathItem({ name, loading }) {
    return (
        <div className={[css.block, css[`loading-${loading}`]].join(' ')}>
            <div className={css.body}>{name}</div>
        </div>
    )
}

export default FilePathItem
