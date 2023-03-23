import css from './fileUploader.module.css'
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import UploadedFile from './uploaded-file';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useId } from 'react';



const FileUploader = () => {
    const id = useId()
    return (
        <div className={css.block}>
            <div className={css.uploaded}>
                <input id={id} type="file" hidden/>
                <label htmlFor={id} className={css.addfile}>
                    <FontAwesomeIcon icon={faPlus} />
                    <span className={css.text}>Добавить файл</span> 
                </label>
                {/* <UploadedFile text='Fissssssssssssssssssssle.txt'/> */}
                {/* <UploadedFile text='Fissssssssssssssssssssle.txt'/>
                <UploadedFile text='Fissssssssssssssssssssle.txt'/> */}
            </div>
            
            
        </div>
    )
}

export default FileUploader;
