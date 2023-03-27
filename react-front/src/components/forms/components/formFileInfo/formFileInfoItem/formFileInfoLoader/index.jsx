import css from '../../formFileInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormFileInfoElem from '../formFileInfoElem';
import { useRequest } from 'hooks/useRequest';
import FormLoader from 'components/forms/components/form-loader';
import FilesService from 'services/filesService';
import { useState, useEffect } from 'react';

const FormFileInfoItem = ({ icon, title, uploading }) => {

    const [getData, isLoading] =  useRequest(
        async (id) => FilesService.get_folder(id, 'blob'),
        {
            200: (response) => {
                setData(URL.createObjectURL(response.data))
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
                <img src={uploadedData} />
            </div>
        </div>
    );
}

export default FormFileInfoItem;
