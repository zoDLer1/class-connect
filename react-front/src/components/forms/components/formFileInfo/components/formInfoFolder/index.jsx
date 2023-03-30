
import { faUser, faCalendarDays, faCircleInfo } from "@fortawesome/free-solid-svg-icons"
import FormInfoItem from "../modules/formInfoItem"

const FormInfoFolder = ({ creationTime, creatorName }) => {
    const items = [
        { title: 'Создатель:', value: creatorName, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]

    return <FormInfoItem icon={faCircleInfo} title='Информация' items={items} />
}

export default FormInfoFolder