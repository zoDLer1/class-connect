import FileBranchItem from "../modules/filebranch-item"

import { DefaultType } from 'types'


const FilebranchNotFound = ({ navigateTo, ...props }) => {
    return <FileBranchItem
        {...props}
        type={DefaultType}
    />
}
export default FilebranchNotFound