import itemCss from '../formInfoItem.module.css'
import css from './formFileInfoWork.module.css'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import FormButton from '../../../../form-button';
import { faTurnDown, faStamp } from '@fortawesome/free-solid-svg-icons';
import FormInput from 'components/forms/components/form-input';
import { MAX_LENGTH_ROOL, NUMER_BETWEEN_ROOL, IS_NUMERIC_ROOL } from 'validation/rools';
import { REQUIRED } from 'validation';
import useForm from 'hooks/useForm';
import FilesService from 'services/filesService';
import { useRequest } from 'hooks/useRequest';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import FormInfoElem from '../formInfoItem/formFileElem';
import { parse_time } from '../../utils';
import { faCalendarDays } from '@fortawesome/free-solid-svg-icons';


const FormFileInfoWork = ({ icon, title, setFilesInfo, id, mark, submitTime, isLate }) => {

    const navigate = useNavigate()

    const [returnWork] = useRequest(
        async (id) => await FilesService.mark(id),
        {
            200: (response) => navigate('/files/' + response.data.guid)
        }
    )



    const { getInput, getSubmit, setInputValue } = useForm({
        mark: {
            value: mark ? mark : '',
            rools: [IS_NUMERIC_ROOL(), NUMER_BETWEEN_ROOL(2, 5), MAX_LENGTH_ROOL(1)],
            validators: [REQUIRED('Поставьте оценку')]
        }
    },
        async (validated_data) => await FilesService.mark(id, validated_data?.mark),
        {
            200: (resp) => setFilesInfo(resp.data)
        }
    )

    useEffect(() => {
        setInputValue('mark', mark ? mark : '')
    }, [mark]);


    return (
        <div className={itemCss.block}>
            <div className={itemCss.header}>
                <div className={itemCss.header_body}>
                    <FontAwesomeIcon color='var(--primary-color)' icon={icon} size='xl' />
                    <h3 className={itemCss.title}>{title}</h3>
                </div>
            </div>
            <div>
                <div className={itemCss.items}>
                    <FormInfoElem title={'Время сдачи:'} icon={faCalendarDays} value={
                        <p className={css[`isLate--${isLate}`]}>{parse_time(submitTime)}</p>
                    } />
                    <div className={css.mark}>
                        <FormInput title='Оценка:' type={'num'} {...getInput('mark')} />
                    </div>
                </div>


                <div className={[itemCss.actions, css.actions].join(' ')}>
                    <div className={css.return}>
                        <FormButton text={'Вернуть работу'} onClick={async () => { await returnWork(id); window.location.reload(); }} icon={faTurnDown} />
                    </div>
                    <FormButton style={1} {...getSubmit()} text={'Поставить оценку'} icon={faStamp} />
                </div>
            </div>
        </div>
    );
}

export default FormFileInfoWork;
