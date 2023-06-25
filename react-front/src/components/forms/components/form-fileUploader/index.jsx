import css from './fileUploader.module.css'
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import UploadedFile from './uploaded-file';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useId } from 'react';
import useFileUploaderInput from 'hooks/useFileUploaderInput';
import { useState, useEffect } from 'react';

const FileUploader = ({ value, validation_methods, error, hidden = false, hideFileAdd = true, ...props }) => {

    const [localHidden, setLocalHidden] = useState(hidden)

    useEffect(()=>{
        setLocalHidden(hidden)
    }, [hidden])

    const id = useId()
    const { getProps, Remove } = useFileUploaderInput({ value, validation_methods })

    if (!localHidden) {
        return (
            <div className={css.block}>
                <h4 className={css.title}>Загрузить:</h4>
                <div className={css.uploaded}>
                    <input id={id}  type="file" {...getProps()} {...props} hidden />
                    <div className={css.files}>
                        {value.map((file, index) => <UploadedFile key={index} text={file.name} onClose={() => Remove(index)} />)}
                    </div>
                    {(!value.length || !hideFileAdd) ?
                        <div className={css.actions}>
                            <label htmlFor={id} className={css.addfile}>
                                <FontAwesomeIcon icon={faPlus} />
                                <span className={css.text}>Добавить файл</span>
                            </label>
                        </div>
                        : null
                    }
                    <span className={css.error}>{error}</span>
                </div>
            </div>
        )
    }
}

export default FileUploader;
