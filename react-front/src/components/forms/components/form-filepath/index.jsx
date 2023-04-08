import css from './form-filepath.module.css'
import FilePathItem from './components/file-path-item'
import FilePathCombiner from './components/filepath-combiner'
import { useMemo } from 'react'


function FormFilePath({ path, loading, loadingMode }) {


    const localPath = useMemo(() => {
        let tempPath = [...path]
        if (tempPath.length > 3) {
            let items = tempPath.splice(1, tempPath.length - 3)
            tempPath.splice(1, 0, items)
        }
        return tempPath
    }, [path])



    return (
        <div className={css.block}>

            {
                localPath.map((item, index)=> Array.isArray(item)
                    ? <FilePathCombiner key={'FPC'} items={item} />
                    : <FilePathItem key={item.guid} loading={loading} {...item} />
                )

            }

        </div>
    )
}

export default FormFilePath
