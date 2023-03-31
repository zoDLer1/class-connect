import FileBranchNavigateTo from "../modules/filebranch-navigateTo"
import { Folder } from "types"

const FileBranchFolder = (props) => {
    
    return <FileBranchNavigateTo
        {...props}
        type={Folder}
    />

}
export default FileBranchFolder