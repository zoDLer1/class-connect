import css from './formFileInfo.module.css'
import { Types, Folder } from './formFileInfoItem/types';

const FormFileInfo = ({ data, name, type, guid }) => {
    const Elem = Types[type?.name] || Types.Folder
    if (data) {
        return (
            <div className={css.block} onClick={evt=>evt.stopPropagation()}>
                <h3 className={css.title}>{name}</h3>
                <div className={css.body}>
                    <Elem {...data} id={guid}/>
                </div>
            </div>
        )
    }
}

export default FormFileInfo;
