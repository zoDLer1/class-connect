import css from './header.module.css'


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
                        <a href="/">Регистрация</a>
                        |
                        <a href="/">Войти</a>
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
