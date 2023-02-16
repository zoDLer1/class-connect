import RegisterForm from "components/forms/register-form"
import pagesCss from '../pages.module.css'


function Register() {
    return (
        <div className={[pagesCss.default_background, pagesCss.content_position_center].join(' ')}>
            <RegisterForm />
        </div>
        
    )
}

export default Register
