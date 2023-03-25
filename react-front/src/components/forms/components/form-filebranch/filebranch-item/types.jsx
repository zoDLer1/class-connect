import React from 'react';
import FileBranchItem from '.';
import { faFolder, faFile, faCube, faUsers } from "@fortawesome/free-solid-svg-icons"


export const Folder = ({ setFolder, value, ...props }) => {
    return <FileBranchItem
        value={value}
        onDoubleClick={async () => await setFolder(value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faFolder}
        {...props}
    />

}


export const Group = ({ setFolder, value, ...props }) => {
    return <FileBranchItem
        value={value}
        onDoubleClick={async () => await setFolder(value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faUsers}
        {...props}
    />

}


export const Subject = ({ setFolder, value, ...props }) => {
    return <FileBranchItem
        value={value}
        onDoubleClick={async () => await setFolder(value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faCube}
        {...props}
    />

}

export const File = ({ setFolder, ...props }) => {
    return <FileBranchItem
        onClick={(evt) => evt.stopPropagation()}
        icon={faFile}
        {...props}
    />

}

export const Task = ({ setFolder, value, ...props }) => {
    return <FileBranchItem
        value={value}
        onDoubleClick={async () => await setFolder(value.guid)}
        onClick={(evt) => evt.stopPropagation()}
        icon={faFile}
        {...props}
    />

}
export const Types = { Folder, Group, Subject, File, Task }