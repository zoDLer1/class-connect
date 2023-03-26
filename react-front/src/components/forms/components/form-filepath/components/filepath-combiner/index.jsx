import css from './filepath-combiner.module.css'
import { useOpen } from 'hooks/useOpen'
import { types } from 'types'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import typesCss from '../../../types.module.css'
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
                                <div className={[css.icon, typesCss[`icon--${item.type.name.toLowerCase()}`]].join(' ')}>
                                    <FontAwesomeIcon icon={types[item.type.name].icon}/>
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
