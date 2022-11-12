import {AuthApiInstanse, DefaultApiInstanse} from '@/api' 


export const AuthAPI = {
    login_url : '/user/login',
    logout_url : '/user/logout',
    register_url: '/user/signup',
    
    login(email, password){
        let data = new FormData()
        data.append('email', email)
        data.append('password', password)
        return AuthApiInstanse.post(this.login_url, data)
    },
    register(data){
        return DefaultApiInstanse.post(this.register_url, data)
    },

    logout(){
        return DefaultApiInstanse.post(this.logout_url)
    }
}