import {AuthAPI} from '@/api/auth' 
import {DefaultApiInstanse} from '@/api' 


export default {
    state:{
        token: null || localStorage.getItem('token'), 
        user: null || localStorage.getItem('user')
    },
    mutations:{
        setToken(state, token){
            state.token = token 
            localStorage.setItem('token', token)
        },
        setUser(state, user){
            state.user = user
            localStorage.setItem('user', user)
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
        onLogin({commit}, {email, password}){
            return AuthAPI.login(email, password).then((res)=>{
                commit('setToken', res.data.acess_token)
                commit('setUser', res.data.username)
                DefaultApiInstanse.interceptors.request.use((config) => {
                    config.headers['authorization'] = `Bearer ${res.data.acess_token}`
                })
            })
        },
        onLogout({ commit }){
            commit('deleteToken')
            commit('deleteUser')
            delete DefaultApiInstanse.defaults.headers['authorization']
        },
        onRegister({ commit }, data){
            commit
            return AuthAPI.register(data).then((res)=>{
                res
            })
        }
    },
    getters:{
        getUser: (state) => state.user
    }
}
// test@test.ru
// test1test