import css from './formFileInfo.module.css'
import { Group } from './formFileInfoItem/types';

const FormFileInfo = ({ data, name }) => {
    if (data) {
        return (
            <div className={css.block}>
                <h3 className={css.title}>{name}</h3>
                <div className={css.body}>
                    <Group info={data.main} />
                </div>
            </div>
        )
    }
}

export default FormFileInfo;
