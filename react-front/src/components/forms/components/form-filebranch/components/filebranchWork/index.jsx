import FileBranchNavigateTo from "../modules/filebranch-navigateTo"
import { Work } from "types"

const FileBranchWork = (props) => {



    return <FileBranchNavigateTo
        type={Work}
        {...props}
    />

}
export default FileBranchWork