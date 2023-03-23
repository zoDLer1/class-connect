import css from './filepath-item.module.css'

function FilePathItem({ guid, setFolder, name, loading }) {
    return (
        <div onClick={()=>setFolder(guid)} className={[css.block, css[`loading-${loading}`]].join(' ')}>
            <div className={css.body}>{name}</div>
        </div>
    )
}

export default FilePathItem
