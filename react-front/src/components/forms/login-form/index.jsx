import formsCss from '../forms.module.css'
import css from './login-form.module.css'
import FormHeader from '../components/form-header'
import FormInput from '../components/form-input'
import Link from 'components/UI/Link'
import FormCheckbox from '../components/form-checkbox'
import FormSubmit from '../components/form-submit'
import useForm from 'hooks/useForm'
import { MAX_LENGTH, IS_EMAIL, REQUIRED, MIN_LENGTH } from 'validation'
import AuthService from 'services/authService'
import { useNavigate } from 'react-router-dom'



function LoginForm() {

    const navigate = useNavigate()



    const { getSubmit, handleServerErrors, getInput } = useForm({
        email: {
            value: '',
            validators: [REQUIRED(), IS_EMAIL(), MAX_LENGTH(50)],

        },
        password: {
            value: '',
            validators: [REQUIRED(), MIN_LENGTH(0)]
        }
    },
        async (data) => await AuthService.login(data),
        {
            200: (response) => navigate('/files/' + response.data.user.folder),
            404: (response) => handleServerErrors(response.response.data.errors)
        }
    )
    return (
        <form className={formsCss.block}>
            <FormHeader text='Вход' />

            <div className={formsCss.inputs}>

                <FormInput title="Почта:" {...getInput('email')} />
                <FormInput title="Пароль:" {...getInput('password')} type='password' />
            </div>

            <div className={[formsCss.group, formsCss.group_between, css.group].join(' ')}>
                <Link text="Забыли пароль?" to='/' size={1} />
                <FormCheckbox id='remember_me' text='Запомнить' />
            </div>
            <FormSubmit {...getSubmit()} text="Войти" />
            <div className={[formsCss.group, formsCss.group_center, css.register_link].join(' ')}>
                <span className={formsCss.text}>Нет аккаунта?</span>
                <Link text="Регистрация" to='/register' size={2} />
            </div>
        </form>
    )
}

export default LoginForm
