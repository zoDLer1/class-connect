import formsCss from '../forms.module.css'
import FormHeader from '../components/form-header';
import FormInput from '../components/form-input';
import FormSubmit from '../components/form-submit';
import FormSelect from '../components/form-select';
import useForm from 'hooks/useForm';
import { REQUIRED, MIN_LENGTH } from 'validation';
import FilesService from 'services/filesService';
import { useLoading } from 'hooks/useLoading';
import { useRequest } from 'hooks/useRequest';
import UsersService from 'services/usersService';
import FormLoader from '../components/form-loader';
import css from './create-form.module.css'
import { useState } from 'react';
import { types } from 'types';
import FileUploader from '../components/form-fileUploader';



const CreateForm = ({ current, close, setFolder }) => {
    const [teachersOptions, setTeachers] = useState([])
    const [ send ] = useRequest(
        async () => await UsersService.teachers(),
        {
            200: (response) => {
                const options = response.data.map(teacher => ({ id: teacher.id, text: [teacher.firstName, teacher.lastName, teacher.patronymic].join(' ') }))
                addInput('teacherId', {
                    value: options[0].id,
                    validators: [REQUIRED()],
                })
                setTeachers(options)
            },
            0: ()=> close()
        }
    )
    const { isLoading } = useLoading(
        async () => {
            if(current.access.includes('Subject') || current.access.includes('Group')){
                await send()
            }
        }
    )

    const { InputsData, getInput, getSubmit, addInput } = useForm({
        name: {
            value: '',
            validators: [REQUIRED(), MIN_LENGTH(4)]
        },
        type: {
            value: current.access[0],
            validators: [REQUIRED()]
        },

    },
        async (validated_data) => FilesService.create(current.guid, validated_data),
        {
            200: () => {
                close()
                setFolder(current.guid)
            }
        }
    )

    const formOptions = [...current.access.map(key => ({ id: key, text: types[key].title }))]
    return (
        <div className={formsCss.block}>
            <FormHeader text='Создать' />
            <div className={css.body}>
                <FormLoader loading={isLoading}>
                    <div className={[formsCss.inputs, css.inputs].join(' ')}>
                        {teachersOptions.length ? <FormSelect {...getInput('teacherId')} options={teachersOptions} title="Преподаватели:" />: ''}
                        <FormSelect {...getInput('type')} options={formOptions} title="Тип:" />
                        
                       
                        
                        <FormInput {...getInput('name')} title="Имя:" />
                        {getInput('type').value === 'File' ?
                            <div className={css.file_uploader}>
                                <FileUploader/>
                            </div> :""}
                    </div>
                    <FormSubmit text="Создать" {...getSubmit()} />
                </FormLoader>
            </div>
        </div>
    );
}

export default CreateForm;
