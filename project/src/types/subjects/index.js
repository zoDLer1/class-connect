import store from '@/store'

export default {
    
    objects: {
        folder: {
            name: 'Папка',
            onCreate_name: 'папку',
            icons:{
                byMimeType:{},
                default: 'folder.svg'
            },
            methods:{
                create:(name, type) => store.dispatch('create', { name, type }),
                read: (guid) => store.dispatch('getData', guid),
                edit: (guid, name) => store.dispatch('edit', { guid, name }),
                delete:(guid, index) =>store.dispatch('delete', { guid, index }),
            },
            typeIndex: 0,
            permissions:{
                create: true,
                read: true,
                edit: true,
                delete: true
            }
        },
        file: {
            name: 'Файл',
            onCreate_name: 'файл',
            icons: {
                byMimeType:{
                    'text/plain':'file.svg',
                    'image/jpeg': 'image.svg',
                    'image/png': 'image.svg',
                },
                default: 'file.svg'
            },
            methods:{
                create:(name, file) => store.dispatch('createFile', { name, file }), 
                read: (guid) => {guid},
                edit: (guid, name) => store.dispatch('edit', { guid, name }),
                delete:(guid, index) =>store.dispatch('delete', { guid, index }),
            },

            typeIndex: 1,
            permissions:{
                create: true,
                read: true,
                edit: true,
                delete: true
            }
        },

        subject:{
            name: 'Предмет',
            onCreate_name: 'предмет',
            icons:{
                byMimeType:{},
                default: 'subject.svg'
            },
            typeIndex: 2,
            permissions:{
                create: true,
                read: true,
                update: false,
                delete: false
            },
            methods:{
                create:(name, type) => store.dispatch('create', { name, type }),
                read: (guid) => store.dispatch('getData', guid),
                edit: (guid, name) => store.dispatch('edit', { guid, name }),
                delete:(guid, index) =>store.dispatch('delete', { guid, index }),
            },
        },

        group: {
            name: 'Группа',
            onCreate_name: 'группу',
            icons:{
                byMimeType:{},
                default: 'group.svg'
            },
            typeIndex: 3,
            permissions:{
                create: true,
                read: true,
                update: false,
                delete: false
            },
            methods:{
                create:(name, type) => store.dispatch('create', { name, type }),
                read: (guid) => store.dispatch('getData', guid),
                edit: (guid, name) => store.dispatch('edit', { guid, name }),
                delete:(guid, index) =>store.dispatch('delete', { guid, index }),
            }
        },

        
        
    },

    global:{
        permissions:{
            create: false,
            read: false,
            edit: true,
            delete: false
        }
    },

    byTypeIndex(index){
        return this.array_objects[index]
    },

    get array_objects(){
        let array = []
        for (let type in this.objects)
            array.splice(this.objects[type].typeIndex, 0, this.objects[type])
        return array
    },
    get KeysTypes(){
        let array = []
        for (let type in this.objects){
            array.push({key: type, value: this.objects[type]})
        }
        return array
    },
    Icon(typeId, mimeType){
        let type = this.array_objects[typeId]
        let path = type.icons.byMimeType[mimeType]
        return require(`#/${path ? path : type.icons.default}`)
    },
    read(typeId){
        return this.array_objects[typeId].methods.read
    } 
    


}