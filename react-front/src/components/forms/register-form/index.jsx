import formsCss from '../forms.module.css'
import css from './register-form.module.css'
import FormHeader from '../components/form-header'
import FormInput from '../components/form-input'
import FormSubmit from '../components/form-submit'
import Link from 'components/UI/Link'



function RegisterForm() {
    return (
        <form className={formsCss.block}>
            <FormHeader text="Регистрация"/>
            <div className={formsCss.inputs}>
                <FormInput title='Имя:'/>
                <FormInput title='Фамилия:'/>
                <FormInput title='Почта:'/>
                <FormInput title='Пароль:'/>
                <FormInput title={<>Повторите <br/> пароль:</>}/>
            </div>
            <FormSubmit id='submit' text='Создать аккаунт'/>
            <div className={[formsCss.group, formsCss.group_center, css.login_link].join(' ')}>
                <span className={formsCss.text}>Уже есть аккаунт?</span>
                <Link text="Войдите" to='/register' size={2}/>
            </div>
        </form>
    )
}

export default RegisterForm
