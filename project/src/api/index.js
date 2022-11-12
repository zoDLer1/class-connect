import axios from "axios";

const DOMAIN = 'https://25.62.130.250:7231'
axios.defaults.timeout = 1000 * 100;

export const Defaultconfig = {
    baseURL: DOMAIN,
    headers:{},
    
}
const token = localStorage.getItem('token')
if (token) Defaultconfig.headers['authorization'] = `Bearer ${token}`


export const DefaultApiInstanse = axios.create(Defaultconfig)

export const AuthApiInstanse = axios.create({
    baseURL: DOMAIN,

})
