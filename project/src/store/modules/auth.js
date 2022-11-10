import {AuthAPI} from '@/api/auth' 
import {DefaultApiInstanse} from '@/api' 


export default {
    state:{
        token: null || localStorage.getItem('token'), 
        userRole: null || localStorage.getItem('userRole')
    },
    mutations:{
        setToken(state, token){
            state.token = token 
            localStorage.setItem('token', token)
        },
        setUserRole(state, userRole){
            state.userRole = userRole
            localStorage.setItem('userRole', userRole)
        },
        deleteToken(state){
            state.token = null
            localStorage.removeItem('token')
        },
        deleteUserRole(state){
            state.userRole = null
            localStorage.removeItem('userRole')
        }
    },
    actions:{
        onLogin({commit}, {email, password}){
            return AuthAPI.login({email, password}).then((res)=>{
                commit('setToken', res.token)
                commit('setUserRole', res.userRole)
                DefaultApiInstanse.interceptors.request.use((config) => {
                    config.headers['authorization'] = `Bearer ${res.token}`
                })
            })
            
        },
        onLogout({ commit }){
            commit('deleteToken')
            commit('deleteUserRole')
            delete DefaultApiInstanse.defaults.headers['authorization']
        }
    }
}