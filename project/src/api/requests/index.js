import {DefaultApiInstanse} from '@/api/' 




export const API = {
    filesystem : '/filesystem',
    group: '/group',
    file: '/filesystem/file',


    show(guid){
        console.log("4");
        return DefaultApiInstanse.get(this.filesystem, {params:{id:guid}})
    },
    create(guid, name, type){
        let formData = new FormData()
        formData.append('name', name)
        formData.append('parentId', guid)
        formData.append('type', type)


        return DefaultApiInstanse.post(this.filesystem, formData)
        
    },
    createFile(data){
        return DefaultApiInstanse.post(this.filesystem, data)
    },

    edit(guid, name){
        return DefaultApiInstanse.patch(this.filesystem, {id:guid, name:name})
    },
    delete(guid){
        return DefaultApiInstanse.delete(this.filesystem, {params:{id:guid}})
    },
    groups(){
        return DefaultApiInstanse.get(this.group)
    }



}