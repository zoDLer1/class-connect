import css from './filepath-combiner.module.css'
import { useOpen } from 'hooks/useOpen'
import Types from 'types'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { useNavigate } from 'react-router-dom'


function FilePathCombiner({ items }) {

    const navigate = useNavigate()

    const { condition, toggle } = useOpen()
    return (
        <>

            <div className={css.block} onClick={toggle}>
                <div className={css.main}>
                    ...
                </div>

            </div>
            {condition &&
                <div className={css.body}>
                    <div className={css.items}>
                        {items.map((item) => {
                            return <div key={item.guid} onClick={() => navigate('/files/'+item.guid)} className={css.item}>
                                <div className={css.icon}>
                                    <FontAwesomeIcon icon={Types[item.type.name].icon} color={Types[item.type.name].iconColor}/>
                                </div>

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
