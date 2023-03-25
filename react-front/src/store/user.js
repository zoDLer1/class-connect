import { makeAutoObservable } from "mobx"
import files from "./files"


class User{
    access = null || localStorage.getItem('access')
    refresh = null || localStorage.getItem('refresh')
    data = null || JSON.parse(localStorage.getItem('user'))

    constructor(){
        makeAutoObservable(this)
    }

    set_access_token(access){
        this.access = access
        localStorage.setItem('access', access)
    }

    set_refresh_token(refresh){
        this.refresh = refresh
        localStorage.setItem('refresh', refresh)
    }

    set_user_data(data){
        this.data = data
        
        files.set_current_folder(data.folder)
        localStorage.setItem('user', JSON.stringify(data))
    } 
}
export default new User()