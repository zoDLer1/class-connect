import {AuthApiInstanse, DefaultApiInstanse} from '@/api' 


export const AuthAPI = {
    login_url : '/user',
    logout_url : '/user/logout',
    
    login(email, password){
        let data = {email, password}
        return AuthApiInstanse.post(this.login_url, data)
    },
    logout(){
        return DefaultApiInstanse.post(this.logout_url)
    }
}