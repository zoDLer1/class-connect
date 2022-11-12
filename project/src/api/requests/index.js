import {DefaultApiInstanse} from '@/api/' 




export const API = {
    filesystem : '/filesystem',
    group: '/group',


    show(guid){
        return DefaultApiInstanse.get(this.filesystem, {params:{id:guid}})
    },
    createFolder(guid, name){
        return DefaultApiInstanse.post(this.filesystem, null, {params:{parentId:guid, name:name}})
    },
    createFile(guid, data){
        return DefaultApiInstanse.post(this.filesystem, data, {params:{parentId:guid}})
    },

    rename(guid, name){
        return DefaultApiInstanse.patch(this.filesystem, {params:{id:guid, name: name}})
    },
    delete(guid){
        return DefaultApiInstanse.delete(this.filesystem, {params:{id:guid}})
    },
    groups(){
        return DefaultApiInstanse.get(this.group)
    }



}