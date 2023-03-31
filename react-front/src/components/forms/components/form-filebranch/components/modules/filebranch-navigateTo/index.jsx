import FileBranchItem from "../filebranch-item"


const FileBranchNavigateTo = ({ navigateTo, value, ...props }) => {

    return <FileBranchItem
        value={value}
        onDoubleClick={ () => navigateTo(value.guid)}
        {...props}
    />

}
export default FileBranchNavigateTo