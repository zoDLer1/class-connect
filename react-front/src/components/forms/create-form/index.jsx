import formsCss from '../forms.module.css'
import FormHeader from '../components/form-header';
import FormInput from '../components/form-input';
import FormSubmit from '../components/form-submit';
import FormSelect from '../components/form-select';
import useForm from 'hooks/useForm';
import { REQUIRED, MIN_LENGTH, IS_EXTANTIONS } from 'validation';
import { MAX_LENGTH_ROOL } from 'validation/rools';
import FilesService from 'services/filesService';
import { useLoading } from 'hooks/useLoading';
import { useRequest } from 'hooks/useRequest';
import UsersService from 'services/usersService';
import FormLoader from '../components/form-loader';
import css from './create-form.module.css'
import { useState } from 'react';
import Types from 'types';
import FileUploader from '../components/form-fileUploader';
import { useEffect } from 'react';
import DateInput from 'components/forms/components/form-dateTimeInput';


const CreateForm = ({ current, close, setFilesInfo }) => {


    const [teachersOptions, setTeachers] = useState([{id: null}])
    const [send, isLoading] = useRequest(
        async () => await UsersService.teachers(),
        {
            200: (response) => {
                const options = response.data.map(teacher => ({ id: teacher.id, text: [teacher.firstName, teacher.lastName, teacher.patronymic].join(' ') }))
                setTeachers(options)
                InputShow('teacherId', String(options[0].id))
            },
            0: () => close()
        }
    )
    useLoading(
        async () => {
            if (current.access.includes('Subject') || current.access.includes('Group')) {
                await send()
            }
        }
    )


    const { InputHide, InputShow, getInput, getSubmit } = useForm({

        teacherId: {
            value: teachersOptions[0].id,
            validators: [REQUIRED()],
            hidden: true,

        },

        name: {
            value: '',
            validators: [REQUIRED()],
        },
        type: {
            value: current.access[0],
            validators: [REQUIRED()]
        },
        uploadedFiles: {
            validators: [MIN_LENGTH(1, 'Загрузите файл')],
            hidden: true,
            value: []
        },
        until: {
            value: ''
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
            return await FilesService.create(current.guid, {...validated_data})

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

    }, [selectConfig.value])


    const formOptions = [...current.access.map(key => ({ id: key, text: Types[key].title }))]
    return (
        <div className={formsCss.block}>
            <FormHeader text='Создать' />
            <div className={css.body}>
                <FormLoader loading={isLoading}>
                    <div className={[formsCss.inputs, css.inputs].join(' ')}>
                        <FormSelect {...getInput('teacherId')} options={teachersOptions} title="Преподаватели:" />
                        <FormSelect {...selectConfig} options={formOptions} title="Тип:" />
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
