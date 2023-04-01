import { faCircleInfo, faCalendarDays, faCubes, faUser, faCube, faGraduationCap, faLink } from '@fortawesome/free-solid-svg-icons';
import FormInfoItem from '../modules/formInfoItem';
import UsersService from 'services/usersService';
import { useRequest } from 'hooks/useRequest';
import FormInfoUser from '../modules/formInfoItem/formInfoUser';


const FormInfoGroup = ({ teacher, creationTime, subjects, students, id, update }) => {

    const [removeStudent] = useRequest(
        async ({ id, current }) => await UsersService.group_remove(id, current),
        {
            200: async () => await update()
        }
    )
    const getSubject = (subject) => {
        return { icon: faCube, title: subject.name }
    }


    const getStudent = (student) => {
        return {
            id: student.id,
            icon: faUser, title:
                student.name,
            action: async (current) => await removeStudent({ id, current })
        }

    }

    const items = [
        { title: 'Преподаватель:', value: [teacher.name, teacher.surname, teacher.patronymic].join(' '), icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays },
        { title: 'Ссылка вступления:', value: `http://localhost:3000/group/enter/${id}`, icon: faLink }
    ]


    return <>
        <FormInfoItem icon={faCircleInfo} title='Информация' items={items} />
        {
            subjects.length
                ? <FormInfoItem
                    icon={faCubes}
                    title='Предметы'
                    items={subjects.map(getSubject)}
                />
                : null
        }
        {
            students.items.length
                ? <FormInfoItem
                    Elem={FormInfoUser}
                    icon={faGraduationCap}
                    title='Студенты'
                    items={students.items.map(getStudent)}
                />
                : null
        }

    </>
}
export default FormInfoGroup