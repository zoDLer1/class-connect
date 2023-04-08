import itemCss from '../formInfoItem.module.css'
import css from './formFileInfoLoader.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useRequest } from 'hooks/useRequest';
import FormLoader from 'components/forms/components/form-loader';
import FilesService from 'services/filesService';
import { useState, useEffect } from 'react';
import { faDownload } from '@fortawesome/free-solid-svg-icons';


const FormFileInfoLoader = ({ icon, title, name, uploading }) => {

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
        <div className={itemCss.block}>
            <div className={itemCss.header}>
                <div className={itemCss.header_body}>
                    <FontAwesomeIcon icon={icon} size='xl' color='var(--primary-color)' />
                    <h3 className={itemCss.title}>{title}</h3>
                </div>
                <a href={url} download={name}>
                    <FontAwesomeIcon color='var(--primary-color)' icon={faDownload} size='lg' cursor={'pointer'} />
                </a>

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
                                        <div className={css.noPreview}>Невозможно отобразить предпросмотр</div>
                    }
                </FormLoader>
            </div>
        </div>
    );
}

export default FormFileInfoLoader;
