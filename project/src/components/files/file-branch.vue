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
    import { mapGetters, mapActions } from 'vuex'
    
    export default{
        components:{
            'filebranch-item': item,
            'menu-component': menu,
        },
        props:{
            data: Array,
        },
        data(){
            return{
                menu:{
                    objs:[
                        {
                            name: 'Новая папка', 
                            icon: this.requireIcon('add_folder.svg'), 
                            function: () => {
                                this.createFolder('name')
                            }    
                        },
                        {
                            name: 'Добавить файл', 
                            icon: this.requireIcon('add_file.svg'), 
                            function: () => {
                                document.querySelector('#file-loader').click()
                            }
                        }
                    ],
                    show: false,
                    coords: [0, 0],
                    offset: [0, 0],
                },
                item_menu:{
                    objs:[
                        {
                            name: 'Переименовать', 
                            icon: this.requireIcon('edit.svg'), 
                            function: () => {
                                
                                // this.remane(this.item_menu.selected, 'newName')
                            }
                        },
                        {
                            
                            name: 'Удалить', 
                            icon: this.requireIcon('delete.svg'), 
                            function: () => {
                                this.item_menu.show = false
                                this.delete({guid:this.item_menu.selected, index:this.item_menu.index})
                            }
                        }
                    ],
                    show: false,
                    coords: [0, 0],
                    offset: [0, 0],
                    selected: null,
                    index: null,
                    
                }
                
            }
        },
        inject: ["requireIcon"],

        computed:mapGetters(['getGuid']),
        methods: {
            ...mapActions(['createFolder', 'createFile', 'delete', 'remname']),
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
                this.createFile(formData)
                
            },
            menu_item_open(guid, index, evt){
                this.item_menu.index = index
                this.item_menu.selected = guid
                this.item_menu.coords = [evt.pageX, evt.pageY]
                this.item_menu.show = !this.item_menu.show
            }
            
        },
        
    }

</script>