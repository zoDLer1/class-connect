import { faBriefcase } from '@fortawesome/free-solid-svg-icons';
import FormFileInfoTask from '../modules/formFileInfoTask';
import user from 'store/user';
import FormInfoItem from '../modules/formInfoItem';
import { useRequest } from 'hooks/useRequest';
import FilesService from 'services/filesService';
import { useState } from 'react';
import { useEffect } from 'react';
import { faUser, faCalendarDays, faCircleInfo } from '@fortawesome/free-solid-svg-icons';
import { parse_time } from '../utils';


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


    useEffect(()=>{
        setWork(props.work)
    }, [props.work])

    const [work, setWork] = useState(props.work);



    if (user.data.role === 'Student') {
        return <FormFileInfoTask title='Ваша работа' {...work} submitTime={work.submitTime ? parse_time(work.submitTime): '--'}
        until={props.until ? parse_time(props.until) : '--' } requests={{ saveFile, removeFile, sendWork }} task_id={props.id} icon={faBriefcase} />
    }
    else{
        const items = [
            { title: 'Создатель:', value: props.creatorName, icon: faUser },
            { title: 'Дата создания:', value: props.creationTime, icon: faCalendarDays },
            { title: 'Срок сдачи:', value: props.until ? parse_time(props.until) : '--',  icon: faCalendarDays  }
        ]

        return <FormInfoItem icon={faCircleInfo} title='Информация' items={items} />
    }


}


export default FormInfoTask
