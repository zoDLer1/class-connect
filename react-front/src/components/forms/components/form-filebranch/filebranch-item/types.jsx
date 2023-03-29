import React from 'react';
import FileBranchItem from '.';
import { faFolder, faFile, faCube, faUsers } from "@fortawesome/free-solid-svg-icons"
import { useNavigate } from 'react-router-dom';

export const Folder = ({ value, ...props }) => {
    const navigate = useNavigate()

    return <FileBranchItem
        value={value}
        onDoubleClick={() => navigate('/files/' + value.guid)}
        icon={faFolder}
        {...props}
    />

}


export const Group = ({  value, ...props }) => {
    const navigate = useNavigate()
    
    return <FileBranchItem
        value={value}
        onDoubleClick={() => navigate('/files/' + value.guid)}
        icon={faUsers}
        {...props}
    />

}


export const Subject = ({ value, ...props }) => {
    const navigate = useNavigate()

    return <FileBranchItem
        value={value}
        onDoubleClick={() => navigate('/files/' + value.guid)}
        icon={faCube}
        {...props}
    />

}

export const File = ({ ...props }) => {
    return <FileBranchItem
        icon={faFile}
        {...props}
    />

}

export const Task = ({ value, ...props }) => {
    const navigate = useNavigate()



    return <FileBranchItem
        value={value}
        onDoubleClick={ () => navigate('/files/' + value.guid)}
        icon={faFolder}
        {...props}
    />

}

export const Work = ({ value, ...props }) => {
    const navigate = useNavigate()



    return <FileBranchItem
        value={value}
        onDoubleClick={ () => navigate('/files/' + value.guid)}
        icon={faFolder}
        {...props}
    />

}

export const Types = { Folder, Group, Subject, File, Task, Work }