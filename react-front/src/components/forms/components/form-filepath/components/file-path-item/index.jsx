import css from './filepath-item.module.css'

function FilePathItem({ name }) {
    return (
        <div className={css.block}>
            <div className={css.body}>{name}</div>
        </div>
    )
}

export default FilePathItem
