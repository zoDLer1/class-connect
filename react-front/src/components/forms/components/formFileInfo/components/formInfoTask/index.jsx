import { faBriefcase } from '@fortawesome/free-solid-svg-icons';
import FormFileInfoTask from '../modules/formFileInfoTask';
import user from 'store/user';
import FormInfoFolder from '../formInfoFolder';
import { useRequest } from 'hooks/useRequest';
import FilesService from 'services/filesService';
import { useState } from 'react';

const FormInfoTask = ({ ...props }) => {

    const [removeFile] = useRequest(
        async (id) => await FilesService.remove(id),
        {
            200: (response) => setWork({ ...work, files: work.files.filter((file) => file.id !== response.config.params.id) })
        })

    const [saveFile] = useRequest(
        async ({ id, file }) => await FilesService.create(id, { uploadedFile: file }),
        {
            200: (res) => setWork(res.data.data.work)
        })


    const [sendWork] = useRequest(
        async (id) => await FilesService.mark(id),
        {
            200: (res) => setWork(res.data.data.work)
        }
    )


    const [work, setWork] = useState(props.work);

    if (user.data.role === 'Student') {
        return <FormFileInfoTask title='Ваша работа' {...work} requests={{ saveFile, removeFile, sendWork }} task_id={props.id} icon={faBriefcase} />
    }
    return <FormInfoFolder {...props} />
}


export default FormInfoTask