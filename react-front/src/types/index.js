import { faFolder, faFile, faCube, faUsers } from "@fortawesome/free-solid-svg-icons"


export const Folder = {
    title: 'Папка',
    icon: faFolder,
    iconColor: 'var(--warning-color)'
}

export const File = {
    title: 'Файл',
    icon: faFile,
    iconColor: 'var(--primary-color)'
}

export const Subject = {
    title: 'Предмет',
    icon: faCube,
    iconColor: 'var(--primary-color)'
}


export const Group = {
    title: 'Группа',
    icon: faUsers,
    iconColor: 'var(--primary-color)'
}

export const Task = {
    title: 'Задание',
    icon: faFolder,
    iconColor: 'var(--orange-color)'
}

export const Work =  {
    title: 'Работа',
    icon: faFolder,
    iconColor: 'var(--orange-color)'
}

export const DefaultType = {
    title: '[Default Type]',
    icon: faFolder,
    iconColor: 'var(--dark-op20-color)'
}
const Types = {
    Folder,
    File,
    Subject,
    Group,
    Task,
    Work,
}


export default Types