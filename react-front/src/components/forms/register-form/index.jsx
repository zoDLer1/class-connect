import formsCss from '../forms.module.css'
import css from './register-form.module.css'
import FormHeader from '../components/form-header'
import FormInput from '../components/form-input'
import FormSubmit from '../components/form-submit'
import Link from 'components/UI/Link'
import useForm from 'hooks/useForm'
import { REQUIRED, IS_EMAIL } from 'validation'
import AuthService from 'services/authService'
import { GlobalUIContext } from 'contexts/GlobalUIContext'
import { useContext } from 'react'
import { useNavigate } from 'react-router-dom'

function RegisterForm() {

    const { alert } = useContext(GlobalUIContext)
    const navigate = useNavigate()

    const { getInput, getSubmit } = useForm({
        name: {
            validators: [REQUIRED()],
            value: ''
        },
        surname: {
            validators: [REQUIRED()],
            value: ''
        },
        patronymic: {
            validators: [],
            value: ''
        },
        email: {
            validators: [REQUIRED(), IS_EMAIL()],
            value: ''
        },
        password: {
            validators: [REQUIRED()],
            value: ''
        },
        passwordConfirm: {
            validators: [REQUIRED()],
            value: ''
        }
    },
        async (validated_data) => AuthService.register(validated_data),
        {
            200: () => {
                alert.show("Вы зарегестрированы")
                navigate('/login')
            }
        }
    )

    return (
        <form className={formsCss.block}>
            <FormHeader text="Регистрация" />
            <div className={formsCss.inputs}>
                <FormInput {...getInput('name')} title='Имя:' />
                <FormInput {...getInput('surname')} title='Фамилия:' />
                <FormInput {...getInput('patronymic')} title='Отчество:' />
                <FormInput {...getInput('email')} title='Почта:' />
                <FormInput {...getInput('password')} title='Пароль:' type='password' />
                <FormInput {...getInput('passwordConfirm')} title={<>Повторите <br /> пароль:</>} type='password' />
            </div>
            <FormSubmit {...getSubmit()} id='submit' text='Создать аккаунт' />
            <div className={[formsCss.group, formsCss.group_center, css.login_link].join(' ')}>
                <span className={formsCss.text}>Уже есть аккаунт?</span>
                <Link text="Войдите" to='/login' size={2} />
            </div>
        </form>
    )
}

export default RegisterForm
