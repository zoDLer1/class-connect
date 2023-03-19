import axios from "axios";
import user from "store/user";
import AuthService from "services/authService";

export const DOMAIN = 'https://localhost:7231'
export const BAESDOMAIN = 'https://localhost:7231'
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
        try {
            if (error.response.status === 401) {

                await AuthService.refresh_token()
                return DefaultApiInstanse.request(error.config)
            }
        }
        catch {
            // console.log(error)
        }
        return Promise.reject(error)
    }
)


export const AuthApiInstanse = axios.create({
    baseURL: DOMAIN,

})

export default DefaultApiInstanse