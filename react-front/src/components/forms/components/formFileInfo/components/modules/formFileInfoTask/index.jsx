import itemCss from '../formInfoItem.module.css'
import css from './formFileInfoTask.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormButton from '../../../../form-button';
import { faPlus, faPaperPlane, faCalendarDays, faStamp } from '@fortawesome/free-solid-svg-icons';
import FormFileInfoTaskFile from './formFileInfoTaskFile';
import { useState } from 'react';
import FormInfoElem from '../formInfoItem/formFileElem';




const FormFileInfoTask = ({ isSubmitted, icon, until, title, guid, isLate, task_id, mark, files, requests }) => {

    const [isLoading, setloading] = useState(false);




    const saveFiles = async (files) => {
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
                <div className={itemCss.header_body}>
                    <FontAwesomeIcon color='var(--primary-color)' icon={icon} size='xl' />
                    <h3 className={itemCss.title}>{title}</h3>
                </div>
            </div>
            <div className={itemCss.task}>
                <div className={itemCss.items}>
                    <FormInfoElem title={'Срок сдачи: '} value={until ? until : '--'} icon={faCalendarDays}/>
                    <FormInfoElem title={'Оценка: '} value={mark ? mark : '--'} icon={faStamp} />
                    
                </div>
                <div className={css.files}>
                    {files?.map(file => <FormFileInfoTaskFile key={file.id} isSubmitted={isSubmitted} remove={() => removeFile(file.id)} {...file} />)}
                </div>
                <div className={itemCss.actions}>
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
