import FormInfoItem from "../modules/formInfoItem"
import { faCircleInfo, faUser, faCalendarDays } from '@fortawesome/free-solid-svg-icons';

const FormInfoSubject = ({ teacher, creationTime }) => {
    const items = [
        { title: 'Преподаватель:', value: teacher.name, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]

    return <FormInfoItem icon={faCircleInfo} title='Информация' items={items} />
}

export default FormInfoSubject