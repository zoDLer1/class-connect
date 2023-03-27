import css from './filebranch-item.module.css'
import { useId } from 'react';
import Input from 'components/UI/Input';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import typesCss from '../../types.module.css'

function FileBranchItem({ value, onMenu, state, actions, icon, ...props }) {

    const id = useId()


    return (

        <div className={[css.block, css[`selected--${state.selected}`]].join(' ')} {...props} onClick={(evt) => { evt.stopPropagation(); actions.select() }} onContextMenu={(evt) => {
            onMenu(evt, value, state.editMode)
        }}>


            <div htmlFor={id} className={css.body}>
                <div className={[css.icon, typesCss[`icon--${value.type.name.toLowerCase()}`]].join(' ')}>
                    <FontAwesomeIcon icon={icon} />
                </div>

                {state.editMode
                    ? <Input style={{ cursor: 'text' }} onChange={(evt) => actions.setProp('name', evt.target.value)} value={value.name} />
                    : <p className={css.title}>{value.name}</p>
                }
            </div>
            <i className={`${css.arrow} fa-solid fa-angle-right`}></i>

        </div>
    )
}

export default FileBranchItem
