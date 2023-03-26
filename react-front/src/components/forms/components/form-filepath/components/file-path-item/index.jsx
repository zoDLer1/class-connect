import css from './filepath-item.module.css'
import { useNavigate } from 'react-router-dom'

function FilePathItem({ guid, name, loading }) {
    const navigate = useNavigate()
    return (
        <div onClick={()=>navigate('/files/'+guid)} className={[css.block, css[`loading-${loading}`]].join(' ')}>
            <div className={css.body}>{name}</div>
        </div>
    )
}

export default FilePathItem
