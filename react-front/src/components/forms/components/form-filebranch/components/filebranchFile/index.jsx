import FileBranchItem from "../modules/filebranch-item"
import { File } from 'types'


const FileBranchFile = ({ navigateTo, ...props }) => {
    return <FileBranchItem
        {...props}
        type={File}
    />
}
export default FileBranchFile