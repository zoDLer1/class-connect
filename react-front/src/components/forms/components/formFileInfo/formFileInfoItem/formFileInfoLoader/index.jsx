import css from '../../formFileInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormFileInfoElem from '../formFileInfoElem';
import { useRequest } from 'hooks/useRequest';
import FormLoader from 'components/forms/components/form-loader';
import FilesService from 'services/filesService';
import { useState, useEffect } from 'react';
import {Buffer} from 'buffer';

const FormFileInfoItem = ({ icon, title, uploading }) => {

    const [getData, isLoading] =  useRequest(
        async (id) => FilesService.get_folder(id),
        {
            200: (response) => {
                console.log()
                
                setData(Buffer.from(response.data, 'base64')) 
                // URL.createObjectURL(new Blob(response.data, {type: "image/jpeg"}))
             
            }
        }
    )
    
    const [uploadedData, setData] = useState()

    useEffect(() => {
        const fetchData = async () => {
            getData(uploading)
        }
        fetchData()
    }, []);
   
    return (
        <div className={css.block}>
            <div className={css.header}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='xl'/>
                </div>
                <h3 className={css.title}>{title}</h3>
            </div>
            <div className={css.items}>
                <FormLoader loading={isLoading}/>
                
                <img src={"data:image/jpeg;base64," + uploadedData} />
                {/* <iframe src={'https://localhost:7231/FileSystem/'+uploading} frameborder="0"></iframe> */}
                {/* {uploading} */}
            </div>
        </div>
    );
}

export default FormFileInfoItem;
