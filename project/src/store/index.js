import { createStore } from 'vuex'
import data from './modules/data.js'
import auth from './modules/auth.js'


export default new createStore({
    actions:{},
    mutations:{},
    state:{},
    getters:{},
    modules:{
        data,
        auth
    }
})