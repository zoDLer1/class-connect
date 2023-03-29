import React from 'react';
import FormFileInfoItem from './formFileInfoItem';
import FormFileInfoLoader from './formFileInfoLoader';
import { faCircleInfo, faUser, faCalendarDays, faCubes, faCube, faGraduationCap, faBriefcase, faLink, faXmark } from '@fortawesome/free-solid-svg-icons';
import FormFileInfoTask from './formFileInfoTask';
import user from 'store/user';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useRequest } from 'hooks/useRequest';
import UsersService from 'services/usersService';

export const Folder = ({ creationTime, creatorName }) => {
    const items = [
        { title: 'Создатель:', value: creatorName, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]

    return <FormFileInfoItem icon={faCircleInfo} title='Информация' items={items} />
}

export const File = ({ creationTime, creatorName, id }) => {
    const items = [
        { title: 'Создатель:', value: creatorName, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]

    return <>
        <FormFileInfoItem icon={faCircleInfo} title='Информация' items={items} />
        <FormFileInfoLoader icon={faCircleInfo} title='Информация' uploading={id} />
    </>
}

export const Group = ({ teacher, creationTime, subjects, students, id, update }) => { // subjects, teacher, students

    const [removeStudent] = useRequest(
        async ({id, current}) => await UsersService.group_remove(id, current),
        {
            200: async () => await update() 
        }
    )

    const items = [
        { title: 'Преподаватель:', value: teacher.name, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays },
        { title: 'Ссылка вступления:', value: `http://localhost:3000/group/enter/${id}`, icon: faLink }
    ]

    const closeIcon = <FontAwesomeIcon icon={faXmark} color='#DB4343' size='lg'/>
    return <>
        <FormFileInfoItem icon={faCircleInfo} title='Информация' items={items} />
        {
            subjects.length ? <FormFileInfoItem icon={faCubes} title='Предметы' items={subjects.map(subject => ({ icon: faCube, title: subject.name }))} /> : null
        }
        {
            students.items.length ? <FormFileInfoItem icon={faGraduationCap}  title='Студенты' items={students.items.map(student => ({ id: student.id, icon: faUser, title: student.name, action: closeIcon, actionFunc: async (current) => await removeStudent({id, current}) }))} /> : null
        }

    </>
}

export const Subject = ({ teacher, creationTime }) => {
    const items = [
        { title: 'Преподаватель:', value: teacher.name, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]


    return <FormFileInfoItem icon={faCircleInfo} title='Информация' items={items} />



}

export const Task = ({ ...props }) => {
    if (user.data.role === 'Student'){
        return <FormFileInfoTask title='Ваша работа' task_id={props.id} icon={faBriefcase} />
    }
    return <Folder {...props} />
}

export const Types = { Group, Folder, Subject, File, Task }
