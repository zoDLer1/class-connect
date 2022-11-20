import {AuthApiInstanse, DefaultApiInstanse} from '@/api' 


export const AuthAPI = {
    login_url : '/user/login',
    teachers_url : '/user/teachers',
    register_url: '/user/signup',
    
    login(email, password){
        let data = new FormData()
        data.append('email', email)
        data.append('password', password)
        return AuthApiInstanse.post(this.login_url, data)
    },
    register(data){
        let formdata = new FormData()

        for (let elem in data){
            formdata.append(elem, data[elem])
        }
        return AuthApiInstanse.post(this.register_url, formdata)
    },
    teachers(){
        return DefaultApiInstanse.get(this.teachers_url)
    },

    logout(){
        return DefaultApiInstanse.post(this.logout_url)
    }
}