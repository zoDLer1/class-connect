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
import { useRequest } from 'hooks/useRequest'
import { useNavigate } from 'react-router-dom'



function LoginForm() {

    const navigate = useNavigate()
    const { send, response } = useRequest(
        async (data) => await AuthService.login(data)
    )

    const onSubmit = async () => {
        const data = getValidatedData()
        if (data !== {}) {
            await send(data, {
                200: () => navigate('/files'),
                404: (response) => changeError('email', response.response.data.errorText)
            })
              
        }
    }

    const { changeError, getInput, getValidatedData } = useForm({
        email: {
            value: 'admin@admin.admin',
            validators: [REQUIRED(), MAX_LENGTH(20), IS_EMAIL()]
        },
        password: {
            value: 'admin',
            validators: [REQUIRED(), MIN_LENGTH(4)]
        }
    },
    )
    return (
        <form className={formsCss.block}>
            <FormHeader text='Вход' />
            {JSON.stringify(response)}
            <div className={formsCss.inputs}>
                <FormInput title="Почта:" {...getInput('email')} />
                <FormInput title="Пароль:" {...getInput('password')} type='password' />
            </div>
            
            <div className={[formsCss.group, formsCss.group_between, css.group].join(' ')}>
                <Link text="Забыли пароль?" to='/' size={1} />
                <FormCheckbox id='remember_me' text='Запомнить' />
            </div>
            <FormSubmit onClick={() => onSubmit()} text="Войти" />
            <div className={[formsCss.group, formsCss.group_center, css.register_link].join(' ')}>
                <span className={formsCss.text}>Нет аккаунта?</span>
                <Link text="Регистрация" to='/register' size={2} />
            </div>
        </form>
    )
}

export default LoginForm
