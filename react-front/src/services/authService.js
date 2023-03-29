import { AuthApiInstanse } from "api";
import user from "store/user";


class AuthService {

    // static 

    static async login(data) {
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

    static async register(data){
        return await AuthApiInstanse.post('/User/signup/', data)
    }

    static async refresh_token() {
        return await AuthApiInstanse.post('/User/refreshToken/', { token: user.refresh }).then(
            (s) => {
                user.set_access_token(s.data.accessToken.token)
                return s
            },
        )
    }

    static async logout() {
        return AuthApiInstanse.post('/users/logout')
    }

}
export default AuthService