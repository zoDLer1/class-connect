import Header from 'components/header'
import css from './index.module.css'


function Index(){
    return (
        <div className={css.block}>
            <Header />
            <div className={css.info}>
                <div className={css.body}>
                    <img src="imgs/logo2.png" alt="" className={css.logo} />
                    <h1 className={css.title}>
                        Московский колледж управления,
                        гостиничного бизнеса и
                        информационных технологий «Царицыно»
                    </h1>
                </div>
            </div>
        </div>
    )
}
export default Index
