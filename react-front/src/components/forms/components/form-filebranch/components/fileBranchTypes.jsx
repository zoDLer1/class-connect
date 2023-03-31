import FileBranchFile from './filebranchFile';
import FileBranchGroup from './filebranchGroup';
import FileBranchSubject from './filebranchSubject';
import FileBranchFolder from './filebranchFolder';
import FileBranchTask from './filebranchTask';
import FileBranchWork from './filebranchWork';


const Types = {
    Folder: FileBranchFolder,
    Group: FileBranchGroup,
    Subject: FileBranchSubject,
    File: FileBranchFile,
    Task: FileBranchTask,
    Work: FileBranchWork
}

export default Types