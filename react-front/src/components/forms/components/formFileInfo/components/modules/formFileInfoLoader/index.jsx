import itemCss from '../formInfoItem.module.css'
import css from './formFileInfoLoader.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useRequest } from 'hooks/useRequest';
import FormLoader from 'components/forms/components/form-loader';
import FilesService from 'services/filesService';
import { useState, useEffect } from 'react';
import { faDownload } from '@fortawesome/free-solid-svg-icons';
import user from 'store/user';


const FormFileInfoLoader = ({ icon, title, name, uploading }) => {


    const [getData, isLoading] = useRequest(
        async (id) => await FilesService.get_folder(id, 'blob'),
        {
            200: async (response) => {
                if (user.currentItem === response.config.params.id){
                    setRendering(false)
                    setType(response.data.type)
                    setText(await response.data.text())
                    setUrl(URL.createObjectURL(response.data))
                }

            }
        }
    )

    const [isRendering, setRendering] = useState(true)
    const [type, setType] = useState()
    const [text, setText] = useState()
    const [url, setUrl] = useState()

    useEffect(() => {
        const fetchData = async () => {
            await getData(uploading)
        }
        fetchData()
        user.set_current_item(uploading)
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
                <FormLoader loading={isLoading || isRendering}>
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
