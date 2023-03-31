import FileBranchNavigateTo from "../modules/filebranch-navigateTo"
import { Task } from "types"


const FileBranchTask = (props) => {

    return <FileBranchNavigateTo
        type={Task}
        {...props}
    />

}
export default FileBranchTask