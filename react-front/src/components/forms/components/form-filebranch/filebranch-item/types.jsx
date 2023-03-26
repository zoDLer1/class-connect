import React from 'react';
import FileBranchItem from '.';
import { faFolder, faFile, faCube, faUsers } from "@fortawesome/free-solid-svg-icons"
import { useNavigate } from 'react-router-dom';

export const Folder = ({ value, ...props }) => {
    const navigate = useNavigate()

    return <FileBranchItem
        value={value}
        onDoubleClick={() => navigate('/files/' + value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faFolder}
        {...props}
    />

}


export const Group = ({ value, ...props }) => {
    const navigate = useNavigate()

    return <FileBranchItem
        value={value}
        onDoubleClick={() => navigate('/files/' + value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faUsers}
        {...props}
    />

}


export const Subject = ({ value, ...props }) => {
    const navigate = useNavigate()

    return <FileBranchItem
        value={value}
        onDoubleClick={() => navigate('/files/' + value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faCube}
        {...props}
    />

}

export const File = ({ ...props }) => {
    return <FileBranchItem
        onClick={(evt) => evt.stopPropagation()}
        icon={faFile}
        {...props}
    />

}

export const Task = ({ value, ...props }) => {
    const navigate = useNavigate()



    return <FileBranchItem
        value={value}
        onDoubleClick={() => navigate('/files/' + value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faFile}
        {...props}
    />

}
export const Types = { Folder, Group, Subject, File, Task }