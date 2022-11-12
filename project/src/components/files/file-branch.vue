<template>
    <div @contextmenu.prevent='menu.toggle' class="files__branch">
        <div class="file-branch">
            <filebranch-item @contextmenu.prevent='(evt) => item_menu.toggle(evt, {selected: item.guid, index: index})' v-for='(item, index) in data' :data='item' :key='index' :id='index+1'></filebranch-item>
            <input @change='Upload' id="file-loader" type="file" hidden>
        </div>
    </div>
    <menu-component :menu='item_menu'></menu-component>
    <menu-component :menu='menu'></menu-component>
</template>

<script>
    import Menu from '@/menu'
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
                menu: new Menu([
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
                    ], 'file-branch'),
                item_menu: new Menu([
                        {
                            name: 'Переименовать', 
                            icon: this.requireIcon('edit.svg'), 
                            function: () => {
                                
                                this.remane(this.item_menu.selected, 'newName')
                            }
                        },
                        {
                            
                            name: 'Удалить', 
                            icon: this.requireIcon('delete.svg'), 
                            function: () => {
                                this.delete({guid:this.item_menu.selected, index:this.item_menu.index})
                            }
                        }
                    ], null)                
            }
        },
        inject: ["requireIcon"],

        computed:mapGetters(['getGuid']),
        methods: {
            ...mapActions(['createFolder', 'createFile', 'delete', 'remname']),
            Upload(evt){
                let formData = new FormData()
                formData.append('uploadedFile', evt.target.files[0])
                this.createFile(formData)
                
            },

        },
        
    }

</script>