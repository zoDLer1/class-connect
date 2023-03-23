import {createRouter, createWebHistory} from 'vue-router'
import index from '@/views/index.vue'
import register from '@/views/register.vue'
import login from '@/views/login.vue'
import subject from '@/views/subject.vue'


const isAuth = true

const authGuard = function(to, from, next){
    if(!isAuth) next({name:'login'});
    else next()
}



const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            name: 'index',
            component: index,
            
        },
        {
            path: '/register',
            name: 'register',
            component: register,
        },
        {
            path: '/login',
            name: 'login',
            component: login,
        },
        {
            path: '/subjects',
            name: 'subjects',
            component: subject,
            beforeEnter: authGuard
        },
    ]
})
export default router;