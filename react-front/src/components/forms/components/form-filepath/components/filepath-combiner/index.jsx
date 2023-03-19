import css from './filepath-combiner.module.css'
import { useOpen } from 'hooks/useOpen'



function FilePathCombiner({ items }) {

    const { condition, toggle } = useOpen() 
    return (
        <>

        <div className={css.block} onClick={toggle}>
            ... 
        </div>
        {condition &&
            <div className={css.body}>
                <div className={css.items}>
                    {items.map((item, index) =>{
                    return  <div key={item.guid} className={css.item}>
                                <i className={`${css.icon} fa-solid fa-folder`}></i>
                                <p className={css.text}>{item.name}</p>  
                            </div>
                    })}
                    
                </div>
            </div>
        }
        </>
        
        
    )
}

export default FilePathCombiner
