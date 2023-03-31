import FileBranchNavigateTo from "../modules/filebranch-navigateTo"
import { Subject } from "types"



const FileBranchSubject = (props) => {


    return <FileBranchNavigateTo
        type={Subject}
        {...props}
    />

}

export default FileBranchSubject