import css from './fileUploader.module.css'
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import UploadedFile from './uploaded-file';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useId } from 'react';
import useFileUploaderInput from 'hooks/useFileUploaderInput';



const FileUploader = ({ value, validate, onChange, error, hidden = false }) => {
    const id = useId()
    const { getProps, Clear } = useFileUploaderInput({ value, validate, onChange })

    if (!hidden) {
        return (
            <div className={css.block}>

                <h4 className={css.title}>Загрузить:</h4>
                <div className={css.uploaded}>
                    <input id={id} type="file" {...getProps()} hidden />

                    {value
                        ? <UploadedFile text={value.name} onClose={Clear} />
                        : <label htmlFor={id} className={css.addfile}>
                            <FontAwesomeIcon icon={faPlus} />
                            <span className={css.text}>Добавить файл</span>
                        </label>
                    }
                    <span className={css.error}>{error}</span>
                </div>


            </div>
        )
    }
}

export default FileUploader;
