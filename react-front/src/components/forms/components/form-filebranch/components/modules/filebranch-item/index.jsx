import css from './filebranch-item.module.css'
import { useId } from 'react';
import Input from 'components/UI/Input';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFolder } from '@fortawesome/free-solid-svg-icons';


function FileBranchItem({ value, onMenu, state, actions, type, ...props }) {
    const id = useId()

    return (
        <div className={[css.block, css[`selected--${state.selected}`]].join(' ')} {...props} onClick={(evt) => { evt.stopPropagation(); actions.select() }} onContextMenu={(evt) => {
            if (value.isEditable) {
                onMenu(evt, value, state.editMode)
            }
        }}>
            <div htmlFor={id} className={css.body}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={type?.icon || faFolder} color={type?.iconColor || 'var(--dark-op20-color)'} />
                </div>
                {state.editMode
                    ? <Input style={{ cursor: 'text' }} onChange={(evt) => actions.setProp('name', evt.target.value)} value={value.name} />
                    : <p className={css.title}>{value.name}</p>
                }
            </div>
            {
                value.type.name !== 'File' &&
                <i className={`${css.arrow} fa-solid fa-angle-right`}></i>

            }
        </div>
    )
}

export default FileBranchItem
