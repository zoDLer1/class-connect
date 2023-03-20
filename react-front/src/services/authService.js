import DefaultApiInstanse, { AuthApiInstanse } from "api";
import user from "store/user";


class AuthService {

    // static 

    static async login(data){
        const response = await AuthApiInstanse.post('/User/login/', data).then(
            (response) => {
                user.set_access_token(response.data.accessToken.token)
                user.set_refresh_token(response.data.refreshToken.token)
                user.set_user_data(response.data.user)
                return response
            }
        )
        return response

        
    }

    static async refresh_token(){
        return DefaultApiInstanse.post('/User/refreshToken/', {refresh:user.refresh}).then(
            (s) => user.access_token(s.data.access),
            
        )
    }

    static async register(email, password){
        return AuthApiInstanse.post('/users/register/', {email, password})
    }
    static async logout(){
        return AuthApiInstanse.post('/users/logout')
    }
    
}
export default AuthService