import css from './formFileInfo.module.css'
import Types from './components/formFileTypes';
import { parse_time } from './components/utils';



const FormFileInfo = ({ data, name, type, setFilesInfo, guid, update }) => {
    const Elem = Types[type?.name] || Types.Folder
    return (
        <div className={css.block} onClick={evt => evt.stopPropagation()} onContextMenu={evt=>evt.stopPropagation()}>
            {data &&
                <>
                    <div className={css.header}>
                        <h3 className={css.title}>{name}</h3>
                    </div>

                    <div className={css.body}>
                        <Elem {...{ ...data, creationTime: parse_time(data.creationTime) }} name={name} setFilesInfo={setFilesInfo} id={guid} update={update} />
                    </div>
                </>

            }

        </div>
    )

}

export default FormFileInfo;
