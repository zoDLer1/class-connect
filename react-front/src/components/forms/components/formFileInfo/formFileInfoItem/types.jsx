import React from 'react';
import FormFileInfoItem from '../formFileInfoItem';
import FormFileInfoLoader from './formFileInfoLoader';
import { faCircleInfo, faUser, faCalendarDays, faCubes, faCube, faGraduationCap } from '@fortawesome/free-solid-svg-icons';


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

export const Group = ({ teacher, creationTime, subjects, students }) => { // subjects, teacher, students
    const items = [
        { title: 'Преподпватель:', value: teacher.name, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]


    return <>
        <FormFileInfoItem icon={faCircleInfo} title='Информация' items={items} />
        {
            subjects.length ? <FormFileInfoItem icon={faCubes} title='Предметы' items={subjects.map(subject => ({ icon: faCube, title: subject.name }))} /> : null
        }
        {
            students.length ? <FormFileInfoItem icon={faGraduationCap} title='Студенты' items={students.items.map(student => ({ icon: faUser, title: student.name }))} /> : null
        }

    </>
}

export const Subject = ({ teacher, creationTime }) => {
    const items = [
        { title: 'Преподпватель:', value: teacher.name, icon: faUser },
        { title: 'Дата создания:', value: creationTime, icon: faCalendarDays }
    ]


    return <FormFileInfoItem icon={faCircleInfo} title='Информация' items={items} />


   
}

export const Task = ({ ...props })=>{
    return <Folder {...props} />
}

export const Types = { Group, Folder, Subject, File, Task }
