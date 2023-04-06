import css from './form-filepath.module.css'
import FilePathItem from './components/file-path-item'
import FilePathCombiner from './components/filepath-combiner'
import { useEffect, useState } from 'react'


function FormFilePath({ path, loading }) {

    const [localPath, setPath] = useState([])

    useEffect(() => {
        let tempPath = [...path]
        if (tempPath.length > 3){
            let items = tempPath.splice(1, tempPath.length-3)
            tempPath.splice(1, 0, items)

        }
        setPath(tempPath)
    }, [path])

    return (
        <div className={css.block}>
            {
                localPath.map((item, index) => Array.isArray(item) ? <FilePathCombiner key={'FPC'}  items={item} /> : <FilePathItem key={item.guid} loading={loading} {...item} />)
            }

        </div>
    )
}

export default FormFilePath
