import css from './form-filebranch.module.css'
import FileBranchItem from './filebranch-item'

function FormFileBranch({ items }) {
    return (
        <div className={css.block}>
            {items.map((item)=><FileBranchItem key={'FBI'+item.guid} data={item}/>)}
        </div>
    )
}

export default FormFileBranch
