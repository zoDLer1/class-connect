import {API} from '@/api/requests'
import DEFAULT_DATA from '@/assets/info.json'


export default {
    actions:{
        getData({ commit }, guid){
            API.show(guid)
                .then(  
                    (response) => {
                        commit('setData', response.data)
                        console.log(response.data)
                    },
                    (error) => {
                        console.log(error.code)
                        commit('setData', DEFAULT_DATA)
                        // if (error.code === 'ECONNABORTED')
                            
                            
                    }
                )
        },
        updateData({ dispatch, getters }){
            dispatch('getData', getters.getGuid)
        },
        createFolder({ commit, getters }, name){
            API.createFolder(getters.getGuid, name).then(  
                (response) => {
                    
                    commit('addItem', response.data)
                },
                (error) => {
                    error
                }
            )
        },
        createFile({ dispatch, getters }, data){
            API.createFile(getters.getGuid, data).then(  
                () => {
                    dispatch('updateData')
                    // commit('addItem', response.data) !!!
                },
                (error) => {
                    error
                }
            )
        },
        delete({ commit }, { guid, index} ){
            API.delete(guid).then(
                () => {
                    commit('deleteItem', index)
                },
                () => {

                }
            )
        },
        // rename({ commit }, guid, name){

        // }
    },
    mutations:{
        deleteItem(state, index){
            
            state.items.splice(index, 1)
        },
        addItem(state, item){
            state.items.push(item)
        },
        setData(state, data){
            state.items = data.items
            state.path = data.path
            state.realPath = data.realPath
            state.guid = data.guid
            localStorage.setItem('guid', data.guid);
        },
        setItems(state, items){
            state.items = items.sort((a, b) => a.type.id - b.type.id)
        },
        setPath(state, path){
            state.path = path
        },
        setRealPath(state, realPath){
            state.realPath = realPath
        },
        setGuid(state, guid){
            state.guid = guid
            localStorage.setItem('guid', guid);
        },
    },
    state:{
        items: [],
        realPath: [],
        guid: null || localStorage.getItem('guid'),
        path: []
    },
    getters:{
        getItems: (state) => state.items,
        getPath: (state) => state.path,
        getRealPath: (state) => state.realPath,
        getGuid: (state) => state.guid
    },
}