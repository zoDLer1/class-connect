<template>
    <div @contextmenu='menu_open' class="files__branch">
        <div class="file-branch">
            <filebranch-item @menuOpen='menu_item_open' v-for='(item, index) in data' :data='item' :key='index' :id='index+1'></filebranch-item>
            
            <input @change='Upload' id="file-loader" type="file" hidden>
        </div>
        <menu-component :menu='item_menu'></menu-component>
        <menu-component :menu='menu'></menu-component>
        
        
        
    </div>
</template>

<script>
    import item from '@/components/files/file_branch/filebranch-item.vue'
    import menu from '@/components/menu/menu-component.vue'
    export default{
        components:{
            'filebranch-item': item,
            'menu-component': menu,
        },
        props:{
            data: Array,
            api: Object,
        },
        data(){
            return{
                menu:{
                    objs:[
                        {
                            name: 'Новая папка', 
                            icon: this.requireIcon('add_folder.svg'), 
                            function: () => {
                                this.api.createFolder(this.menu.selected, 'name')
                                .then((response) => {
                                    console.log(response)
                                });
                                this.UpdateData()
                            }
                            
                        },
                        {
                            name: 'Добавить файл', 
                            icon: this.requireIcon('add_file.svg'), 
                            function: () => {
                                let loader = document.querySelector('#file-loader')
                                loader.click()
                            }
                        }
                    ],
                    show: false,
                    coords: [0, 0],
                    offset: [0, 0],
                    selected: this.guid
                },
                item_menu:{
                    objs:[
                        {
                            name: 'Переименовать', 
                            icon: this.requireIcon('edit.svg'), 
                            function: (evt) => {
                                return evt
                            }
                        },
                        {
                            name: 'Удалить', 
                            icon: this.requireIcon('delete.svg'), 
                            function: () => {
                                this.api.createFolder.delete(this.guid)
                                .then((response) => {
                                    console.log(response)
                                });
                                this.UpdateData()
                            }
                        }
                    ],
                    show: false,
                    coords: [0, 0],
                    offset: [0, 0],
                    selected: null
                    
                }
                
            }
        },
        inject: ["requireIcon", "guid", "UpdateData"],
        methods: {
            menu_open(evt){
                evt.preventDefault();
                if (evt.target.classList.contains('file-branch')){
                    this.menu.coords = [evt.pageX, evt.pageY]
                    this.menu.show = !this.menu.show
                }   
                else{
                    this.menu.show = false
                }
            },
            Upload(evt){
                let formData = new FormData()
                formData.append('uploadedFile', evt.target.files[0])
                console.log(formData)
                this.api.createFile(this.guid, formData)
                .then((response) => {
                    console.log(response)
                });
            },
            menu_item_open(id, evt){
                this.item_menu.selected = id
                this.item_menu.coords = [evt.pageX, evt.pageY]
                this.item_menu.show = !this.item_menu.show
            }
            
        },
        
    }

</script>