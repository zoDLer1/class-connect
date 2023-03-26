import css from './header.module.css'
import { Link } from 'react-router-dom'

function Header() {
    const user = false
    return (
        <div className={css.block}>
            <div className={css.links}>
                <div className={css.link}>
                    {user && <a href="/">Моя группа</a>}
                        
                </div>

                <div className={css.link}>
                    {!user 
                    ? 
                    <>
                        <Link to="/register">Регистрация</Link>
                        |
                        <Link to="/login">Войти</Link>
                    </>
                        
                    :
                        <a href="/">{user.name}</a>
                    }
                </div>
            </div>
        </div>
    )
}

export default Header
