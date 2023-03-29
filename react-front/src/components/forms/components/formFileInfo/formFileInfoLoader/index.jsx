import css from '../formFileInfoItem.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useRequest } from 'hooks/useRequest';
import FormLoader from 'components/forms/components/form-loader';
import FilesService from 'services/filesService';
import { useState, useEffect } from 'react';

const FormFileInfoItem = ({ icon, title, uploading }) => {

    const [getData, isLoading] = useRequest(
        async (id) => FilesService.get_folder(id, 'blob'),
        {
            200: async (response) => {
                setType(response.data.type)
                setText(await response.data.text())
                setUrl(URL.createObjectURL(response.data))
            }
        }
    )

    const [type, setType] = useState()
    const [text, setText] = useState()
    const [url, setUrl] = useState()

    useEffect(() => {
        const fetchData = async () => {
            getData(uploading)
        }
        fetchData()
    }, [uploading]);

    return (
        <div className={css.block}>
            <div className={css.header}>
                <div className={css.icon}>
                    <FontAwesomeIcon icon={icon} size='xl' />
                </div>
                <h3 className={css.title}>{title}</h3>
            </div>
            <div className={css.image}>
                <FormLoader loading={isLoading}>
                    {
                        type?.startsWith('image')
                        ?
                            <img src={url} />
                        : type?.startsWith('text') && text?.length < 5000
                        ?
                            <pre>{text}</pre>
                        : type?.startsWith('application/mp4') || type?.startsWith('video/mp4') 
                        ?
                            <video src={url} controls />
                        : type?.startsWith('application/pdf') 
                        ?
                            <iframe title="Preview" src={url} />
                        :
                        <div>Невозможно отобразить предпросмотр</div>
                    }
                </FormLoader>
            </div>
        </div>
    );
}

export default FormFileInfoItem;
