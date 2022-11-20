import {API} from '@/api/requests'
import DEFAULT_DATA from '@/assets/info2.json'


export default {
    actions:{
        getData({ commit }, guid){
            
            return API.show(guid)
                .then(  
                    (response) => {
                        commit('setData', response.data)
                        console.log(response.data)
                    },
                    (error) => {
                        console.log(error);
                        commit('setData', DEFAULT_DATA)
                        // if (error.code === 'ECONNABORTED')
                            
                            
                    }
                )
        },
        updateData({ dispatch, getters }){
            dispatch('getData', getters.getGuid)
        },
        create({ commit, getters }, { name, type }){
            
            return API.create(getters.getGuid, name, type).then(  
                (response) => {
                    
                    commit('addItem', response.data)
                },
                
            )
        },
        createFile({ dispatch, getters }, { name, file }){
            let formData = new FormData()
            formData.append('uploadedFile', file)
            formData.append('id', getters.getGuid)
            formData.append('name', name)

            API.createFile(formData).then(  
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
        edit({ commit }, { guid, name }){
            commit
            return API.edit(guid, name).then(()=>{

            })
            // !!!
        }
    },
    mutations:{
        deleteItem(state, index){
            
            state.items.splice(index, 1)
        },
        
        addItem(state, item){
            state.items.push(item)
        },
        setData(state, data){
            state.children = data.children
            state.path = data.path
            state.realPath = data.realPath
            state.guid = data.guid
            localStorage.setItem('guid', data.guid);
        },
        setItems(state, items){
            state.children = items.sort((a, b) => a.type.id - b.type.id)
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
        deleteGuid(state){
            state.guid = null
            localStorage.removeItem('guid')
        },
    },
    state:{
        children: [],
        realPath: [],
        guid: null || localStorage.getItem('guid'),
        path: []
    },
    getters:{
        getItems: (state) => state.children,
        getPath: (state) => state.path,
        getRealPath: (state) => state.realPath,
        getGuid: (state) => state.guid
    },
}