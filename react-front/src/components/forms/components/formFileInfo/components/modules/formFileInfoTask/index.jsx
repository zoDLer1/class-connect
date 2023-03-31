import itemCss from '../formInfoItem.module.css'
import css from './formFileInfoTask.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormButton from '../../../../form-button';
import { faPlus, faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import FormFileInfoTaskFile from './formFileInfoTaskFile';
import { useState } from 'react';
import { useContext } from 'react';
import { GlobalUIContext } from 'contexts/GlobalUIContext';




const FormFileInfoTask = ({ isSubmitted, icon, title, guid, task_id, files, requests }) => {

    const [isLoading, setloading] = useState(false);

    const { alert } = useContext(GlobalUIContext)




    const saveFiles =  async (files) => {
        setloading(true)
        for (const file of files) {
            await requests.saveFile({ id: task_id, file })
        }
        setloading(false)
    }

    const removeFile = (id) => {
        requests.removeFile(id)
    }

    return (
        <div className={itemCss.block}>

            <div className={itemCss.header}>
                <div className={itemCss.icon}>
                    <FontAwesomeIcon icon={icon} size='xl' />
                </div>
                <h3 className={itemCss.title}>{title}</h3>
            </div>
            <div className={itemCss.task}>
                <div className={css.files}>
                    {files?.map(file => <FormFileInfoTaskFile key={file.id} isSubmitted={isSubmitted} remove={() => removeFile(file.id)} {...file} />)}
                </div>
                <div className={css.actions}>
                    {!isSubmitted &&
                        <>
                            <label className={css.addFile} htmlFor="fileUploader">
                                <FormButton text={'добавить файл'} icon={faPlus} />
                            </label>
                            <input multiple onChange={(evt) => { saveFiles(evt.target.files) }} type="file" id='fileUploader' hidden disabled={isLoading} />
                          
                        </>  
                    }
                    <div className={[css.sendWork, css[`isSubmitted--${isSubmitted}`]].join(' ')}>
                        <FormButton style={1} text={isSubmitted ? 'Отменить отправку' : 'сдать работу'} onClick={() => requests.sendWork(guid)} disabled={isLoading} icon={faPaperPlane} />
                    </div>

                </div>

            </div>


        </div>
    );
}

export default FormFileInfoTask;
