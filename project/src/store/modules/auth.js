import {AuthAPI} from '@/api/auth' 
import {DefaultApiInstanse} from '@/api' 


export default {
    state:{
        token: null || localStorage.getItem('token'), 
        user: null || JSON.parse(localStorage.getItem('user')),
        teachers: null || JSON.parse(localStorage.getItem('teachers'))
    },
    mutations:{

        setTeachers(state, teachers){
            state.teachers = teachers
            localStorage.setItem('teachers', JSON.stringify(teachers))
        },

        setToken(state, token){
            state.token = token 
            localStorage.setItem('token', token)
        },
        setUser(state, user){
            state.user = user
            localStorage.setItem('user', JSON.stringify(user))
        },
        deleteToken(state){
            state.token = null
            localStorage.removeItem('token')
        },
        deleteUser(state){
            state.user = null
            localStorage.removeItem('user')
        }
    },
    actions:{
        onLogin({ commit, dispatch }, {email, password}){
            AuthAPI.login(email, password).then((res)=>{
                console.log(res.data.accessToken.token)
                commit('setToken', res.data.accessToken.token)
                commit('setUser', res.data.user)
                DefaultApiInstanse.interceptors.request.use((config) => {
                    config.headers['authorization'] = `Bearer ${res.data.accessToken.token}`
                    return config
                })
                dispatch('getData', res.data.user.folder);
            })
        },
        onLogout({ commit }){
            commit('deleteToken')
            commit('deleteUser')
            commit('deleteGuid')
            delete DefaultApiInstanse.defaults.headers['authorization']
        },
        onRegister({ dispatch }, data){
            
            return AuthAPI.register(data).then(
                ()=>{
                    dispatch('onLogin', {email: data.email, password: data.password})
                }
                
            )
        },
        Teachers({ commit }){
            return AuthAPI.teachers().then(
                (res) => {
                    commit('setTeachers', res.data)
                }
            )
        }
    },
    getters:{
        getUser: (state) => {
            return state.user
        },
        getTeachers(state){
            return state.teachers
        }
    },
}
// test@test.ru
// test1test