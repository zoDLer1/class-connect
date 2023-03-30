import { faCircleInfo, faUser, faCalendarDays, faCubes, faCube, faGraduationCap, faLink, faXmark } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormInfoItem from '../modules/formInfoItem';
import UsersService from 'services/usersService';
import { useRequest } from 'hooks/useRequest';



const FormInfoGroup = ({ teacher, creationTime, subjects, students, id, update }) => {

    const [removeStudent] = useRequest(
        async ({ id, current }) => await UsersService.group_remove(id, current),
        {
            200: async () => await update()
        }
    )

    const items = [
        { title: 'Преподаватель:', value: teacher.name, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays },
        { title: 'Ссылка вступления:', value: `http://localhost:3000/group/enter/${id}`, icon: faLink }
    ]

    const closeIcon = <FontAwesomeIcon icon={faXmark} color='#DB4343' size='lg' />
    return <>
        <FormInfoItem icon={faCircleInfo} title='Информация' items={items} />
        {
            subjects.length ? <FormInfoItem icon={faCubes} title='Предметы' items={subjects.map(subject => ({ icon: faCube, title: subject.name }))} /> : null
        }
        {
            students.items.length ? <FormInfoItem icon={faGraduationCap} title='Студенты' items={students.items.map(student => ({ id: student.id, icon: faUser, title: student.name, action: closeIcon, actionFunc: async (current) => await removeStudent({ id, current }) }))} /> : null
        }

    </>
}
export default FormInfoGroup