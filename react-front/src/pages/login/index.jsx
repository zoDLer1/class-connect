import css from './login.module.css'
import LoginForm from 'components/forms/login-form'

function Login() {
    return (
        <div className={css.block}>
            <LoginForm />
        </div>
    )
}

export default Login
