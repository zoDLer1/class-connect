import LoginForm from 'components/forms/login-form'
import pagesCss from '../pages.module.css'

function Login() {
    return (
        <div className={[pagesCss.default_background, pagesCss.content_position_center].join(' ')}>
            <LoginForm />
        </div>
    )
}

export default Login
