import css from './formFileInfo.module.css'
import Types from './components/types';
import { parse_time } from './components/utils';


const FormFileInfo = ({ data, name, type, guid, setFolder }) => {
    const Elem = Types[type?.name] || Types.Folder
    return (
        <div className={css.block} onClick={evt => evt.stopPropagation()}>
            {data &&
                <>
                    <h3 className={css.title}>{name}</h3>
                    <div className={css.body}>
                        <Elem {...{...data, creationTime: parse_time(data.creationTime)}} id={guid} update={() => setFolder(guid)} />
                    </div>
                </>

            }
        </div>
    )

}

export default FormFileInfo;
