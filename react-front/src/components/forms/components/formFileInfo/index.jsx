import css from './formFileInfo.module.css'
import { Types, Folder } from './types';

const FormFileInfo = ({ data, name, type, guid, setFolder }) => {
    const Elem = Types[type?.name] || Types.Folder

    return (
        <div className={css.block} onClick={evt => evt.stopPropagation()}>
            {data &&
                <>
                    <h3 className={css.title}>{name}</h3>
                    <div className={css.body}>
                        <Elem {...data} id={guid} update={()=>setFolder(guid)} />
                    </div>
                </>

            }
        </div>
    )

}

export default FormFileInfo;
