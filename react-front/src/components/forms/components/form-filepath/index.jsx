import css from './form-filepath.module.css'
import FilePathItem from './components/file-path-item'
import FilePathSeporator from './components/filepath-seporator'
import FilePathCombiner from './components/filepath-combiner'
import { useEffect, useState } from 'react'

function FormFilePath({ path }) {

    const [localPath, setPath] = useState([])
    
    useEffect(()=> {
        let tempPath = [...path]
        if (tempPath.length > 3){
            let items = tempPath.splice(2, tempPath.length-3)
            tempPath.splice(2, 0, items)
            
        }
        setPath(tempPath)
    }, [path])
    

    
 


    return (
        <div className={css.block}>
            {
                localPath.map((item, index) => {
                    let result = null
                    if (Array.isArray(item))
                        result = <FilePathCombiner items={item} />
                    else
                        result = <FilePathItem  {...item} />
            
                    return <>
                        {result}
                        {localPath.length-1 !== index 
                        ? <FilePathSeporator />
                        : ''}
                    </>
            
                })
            }
            
        </div>
    )
}

export default FormFilePath
