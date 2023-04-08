import FormInfoItem from '../modules/formInfoItem';
import { faCircleInfo, faUser, faCalendarDays } from '@fortawesome/free-solid-svg-icons';
import FormFileInfoLoader from "../modules/formFileInfoLoader";




const FormInfoFile = ({ creationTime, name, creatorName, id }) => {
    const items = [
        { title: 'Создатель:', value: creatorName, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]

    return <>
        <FormInfoItem icon={faCircleInfo} title='Информация' items={items} />
        <FormFileInfoLoader icon={faCircleInfo}  name={name} title='Содержание' uploading={id} />
    </>
}

export default FormInfoFile
