import Header from 'components/header'
import css from './index.module.css'

function Index(){
    return (
        <div className={css.block}>
            <Header />
            <div className={css.info}>
                <div className={css.body}>
                    <img src="imgs/logo.png" alt="" className={css.logo} />
                    <svg width="5" height="138" viewBox="0 0 5 138" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M2.5 0.25V137.75" stroke="white" strokeWidth="5"/>
                    </svg>
                    <h1 className={css.title}>
                        Национальный<br />
                        исследовательский<br />
                        ядерный университет «МИФИ»
                    </h1>
                </div>
        
                <div className={css.links}>
                    <a href="/" className={css.link}>История кафедры</a>
                </div>
            </div>
        </div>
    )
}
export default Index
