import css from '../formFileInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FileUploader from '../../form-fileUploader';
import useForm from 'hooks/useForm';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import FormButton from '../../form-button';
import { useRequest } from 'hooks/useRequest';
import { MIN_LENGTH } from 'validation';
import FilesService from 'services/filesService';

const FormFileInfoTask = ({ icon, title, task_id }) => {


    const [saveFile] = useRequest(
        async ({id, data}) => FilesService.create(id, {uploadedFiles: data}),
        {
            200: (res) => console.log(res)
        })


    const { getInput, getSubmit } = useForm(
        {
            uploadedFiles: {

                validators: [MIN_LENGTH(1, 'Загрузите файл')],
                value: []
            },

        },
        async (validated_data) => {

            for (const file of validated_data.uploadedFiles) {
               await saveFile({id: task_id, data: [file]})
            }
            
        })

    return (
        <div className={css.block}>

            <div className={css.header}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='xl' />
                </div>
                <h3 className={css.title}>{title}</h3>
            </div>
            <div className={css.task}>
                <FileUploader displayLabel={false} hideFileAdd={false} filesDisplay={'grid-3-col'} {...getInput('uploadedFiles')} actions={<FormButton {...getSubmit()} icon={faPaperPlane} text={'сдать работу'} style={1} />} multiple />
            </div>


        </div>
    );
}

export default FormFileInfoTask;
