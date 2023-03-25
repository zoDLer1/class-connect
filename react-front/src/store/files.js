import { makeAutoObservable } from "mobx"

class Files{
    current_folder = null || localStorage.getItem('current_folder')


    constructor(){
        makeAutoObservable(this)
    }

    set_current_folder(current_folder){
        this.current_folder = current_folder
        localStorage.setItem('current_folder', current_folder)
    }


}
export default new Files()