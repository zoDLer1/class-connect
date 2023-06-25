import formsCss from '../forms.module.css'
import FormHeader from '../components/form-header';
import FormInput from '../components/form-input';
import FormSubmit from '../components/form-submit';
import FormSelect from '../components/form-select';
import useForm from 'hooks/useForm';
import { REQUIRED, MIN_LENGTH, IS_EXTANTIONS, DATE_IS_FUTURE } from 'validation';
import FilesService from 'services/filesService';
import UsersService from 'services/usersService';
import FormLoader from '../components/form-loader';
import css from './create-form.module.css'
import Types from 'types';
import FileUploader from '../components/form-fileUploader';
import { useEffect } from 'react';
import DateInput from 'components/forms/components/form-dateTimeInput';


const CreateForm = ({ current, close, setFilesInfo }) => {



    const { InputHide, InputShow, getInput, getSubmit } = useForm({

        teacherId: {
            value: '',
            validators: [REQUIRED()],
            options: {
                values: [],
                asyncLoadOptions:  UsersService.teachers,
                mapping: (value) => ({id: value.id, text: `${value.firstName} ${value.lastName} ${value.patronymic}`})
            }
        },

        name: {
            value: '',
            validators: [REQUIRED()],
        },
        type: {
            value: current.access[0],
            validators: [REQUIRED()],
            options: {
                values: [...current.access.map(key => ({ id: key, text: Types[key].title }))]
            }
        },
        uploadedFiles: {
            validators: [MIN_LENGTH(1, 'Загрузите файл')],
            hidden: true,
            value: []
        },
        until: {
            value: '',
            hidden: true,
            // validators: [DATE_IS_FUTURE()]
        }


    },
        async (validated_data) => {
            if (validated_data.uploadedFiles?.length){
                let response = null
                for (const file of validated_data.uploadedFiles){
                    response = await FilesService.create(current.guid, {...validated_data, uploadedFile: file})
                }
                return response

            }
            return await FilesService.create(current.guid, {...validated_data, until: validated_data.until? validated_data.until.toISOString(): null})

        },
        {
            200: (response) => {
                setFilesInfo(response.data)
                close()

            }
        }
    )

    const selectConfig = getInput('type')


    useEffect(()=>{
        if (selectConfig.value === 'File' || selectConfig.value === 'Work'){
            InputShow('uploadedFiles')
            InputHide('name')
        }
        else{
            InputHide('uploadedFiles')
            InputShow('name')
        }

        selectConfig.value === 'Group'  || selectConfig.value === 'Subject' ?  InputShow('teacherId') : InputHide('teacherId')
        selectConfig.value === 'Task' ? InputShow('until') : InputHide('until')

    }, [selectConfig.value])



    return (
        <div className={formsCss.block}>
            <FormHeader text='Создать' />
            <div className={css.body}>
                <FormLoader loading={false}>
                    <div className={[formsCss.inputs, css.inputs].join(' ')}>
                        <FormSelect {...selectConfig} title="Тип:" />
                        <FormSelect {...getInput('teacherId')} title="Преподаватель:" />
                        <FormInput {...getInput('name')} title="Имя:" />
                        <FileUploader {...getInput('uploadedFiles')} multiple hideFileAdd={false} />
                        <DateInput {...getInput('until')}/>
                    </div>
                    <FormSubmit text="Создать" {...getSubmit()} />
                </FormLoader>
            </div>
        </div>
    );
}

export default CreateForm;
