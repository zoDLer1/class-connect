import axios from "axios";
import user from "store/user";
import AuthService from "services/authService";




export const DOMAIN = 'https://localhost:7231'
// axios.defaults.timeout = 1000 * 100;

export const Defaultconfig = {
    baseURL: DOMAIN,
    headers: {},

}

const DefaultApiInstanse = axios.create(Defaultconfig)

DefaultApiInstanse.interceptors.request.use((config) => {
    config.headers.Authorization = `Bearer ${user.access}`
    return config
})

DefaultApiInstanse.interceptors.response.use(
    (config) => {
        return config
    },
    async (error) => {
        
        if (error.response.status === 401) {
            try{
                await AuthService.refresh_token()
                return DefaultApiInstanse.request(error.config)
            }
            catch{
                return error
            }
        }
        
        
        
        return error
    }
)


export const AuthApiInstanse = axios.create({
    baseURL: DOMAIN,

})

export default DefaultApiInstanse