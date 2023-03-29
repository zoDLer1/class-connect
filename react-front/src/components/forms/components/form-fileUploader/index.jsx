import css from './fileUploader.module.css'
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import UploadedFile from './uploaded-file';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useId } from 'react';
import useFileUploaderInput from 'hooks/useFileUploaderInput';



const FileUploader = ({ value, validate, onChange, error, hidden = false, hideFileAdd = true, displayLabel = true, actions, filesDisplay = 'column', ...props }) => {
    const id = useId()
    const { getProps, Remove } = useFileUploaderInput({ value, validate, onChange })

    if (!hidden) {
        return (
            <div className={css.block}>
                {displayLabel && <h4 className={css.title}>Загрузить:</h4>}

                <div className={css.uploaded}>
                    <input id={id} type="file" {...getProps()} {...props} hidden />

                    <div className={css[`files--${filesDisplay}`]}>
                        {value.map((file, index) => <UploadedFile key={index} text={file.name} onClose={() => Remove(index)} />)}


                    </div>
                    {(!value.length || !hideFileAdd) ?
                        <div className={css.actions}>
                            <label htmlFor={id} className={css.addfile}>
                                <FontAwesomeIcon icon={faPlus} />
                                <span className={css.text}>Добавить файл</span>
                            </label>
                            {actions}
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
