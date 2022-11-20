<template>
    <div @contextmenu.prevent='menu.toggle' class="files__branch">
        <div class="file-branch">
            <filebranch-item 
            @edit='edit_subj'
            @contextmenu.prevent='(evt) => item_menu.toggle(evt, {selected: item, index: index})' 
            v-for='(item, index) in filteredData' 
            :data='item' 
            :key='index' 
            :id='index+1'
            :edit='editable[index]'
            >
            </filebranch-item>
            
        </div>
    </div>
    <itemMenu-component :menu='item_menu'></itemMenu-component>
    <menu-component :menu='menu'></menu-component>
    <subject-popup @create='create_subj' @close='popupToggle' v-if='popup'></subject-popup>
</template>

<script>
    import { MainMenu, ItemMenu } from '@/menu/main'
    import item from '@/components/files/file_branch/filebranch-item.vue'
    import menu from '@/components/menu/menu-component.vue'
    import itemMenu from '@/components/menu/itemMenu-component.vue'
    import popup from '@/components/popup/subject-popup.vue'
    import { mapActions } from 'vuex'
    
    export default{
        components:{
            'filebranch-item': item,
            'menu-component': menu,
            'subject-popup': popup,
            'itemMenu-component': itemMenu
        },
        props:{
            data: Array,
        },
        inject:['types', 'alert'],
        data(){
            return{
                popup: false,
                editable: Array(this.data.length),
                menu: new MainMenu({
                    create:{
                        name: 'Создать', 
                        icon: 'add_folder.svg', 
                        func: () => {
                            this.popupToggle()
                        }    
                    }
                }, 'file-branch'),
                

                item_menu: new ItemMenu({
                    edit:{
                        name: 'Переименовать', 
                        icon: 'edit.svg', 
                        func: () => {
                            this.EditMode(true)
                            this.item_menu.objs.edit.setCondition(false)
                            this.item_menu.objs.save.setCondition(true)
                        },

                
                    },
                    save:{
                        name: 'Сохранить', 
                        icon: 'save.svg', 
                        func: () => {
                            this.EditMode(false)
                            this.item_menu.objs.edit.setCondition(true) 
                            this.item_menu.objs.save.setCondition(false) 
                            
                        },
                        condition: false
                
                    },
                    delete:{
                        name: 'Удалить', 
                        icon: 'delete.svg', 
                        func: () => {
                            let item = this.item_menu.selected
                            this.delete_subj(item)                            
                        },

                    }
                }),                
            }
        },
        computed:{
            filteredData(){
                return this.data.filter(item => this.types.byTypeIndex(item.type.id-1).permissions.read)
            }
        },

        methods: {
            ...mapActions(['createFolder', 'createFile', 'delete', 'remname']),       
            popupToggle(){
                this.popup = !this.popup
            },
            // https://stackoverflow.com/questions/1026069/how-do-i-make-the-first-letter-of-a-string-uppercase-in-javascript
            capitalizeFirstLetter(string) {
                return string.charAt(0).toUpperCase() + string.slice(1);
            },
            create_subj(key, name, file){
                
                let object = this.types.objects[key]
                if (object.permissions.create){
                    object.methods.create(name, file ? file : this.capitalizeFirstLetter(key)).then(
                        (response) => console.log(response),
                        (error) => {console.log(error); this.alert(`${error.response.status}: ${error.response.data.errorText}`)}
                    )
                }
                else{
                    this.alert('У вас недостаточно прав!')
                }
                this.popupToggle()
            },

            edit_subj(item, name){
                this.item_menu.objs.save.func()
                let object = this.types.byTypeIndex(item.type.id-1)
                if (object.permissions.edit){
                 
                    object.methods.edit(item.guid, name)
                }
                else{
                    this.alert('У вас недостаточно прав!')
                }

            },
            delete_subj(item){
                let object = this.types.byTypeIndex(item.type.id-1)
                if (object.permissions.delete){
                    object.methods.delete(item.guid, this.item_menu.index)
                }
                else{
                    this.alert('У вас недостаточно прав!')
                }
            },
            EditMode(condition){
                this.editable[this.item_menu.index] = condition
            },

        },
       
        
    }

</script>