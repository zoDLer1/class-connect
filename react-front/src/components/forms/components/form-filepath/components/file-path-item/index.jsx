import css from './filepath-item.module.css'

function FilePathItem({ name }) {
    return (
        <h3 className={css.block}>
            {name}   
        </h3>
    )
}

export default FilePathItem
