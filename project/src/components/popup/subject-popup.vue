<template>
    <div class="popup">
        <form @submit.prevent='onSubmit' action="" class="form">
        <div class="form__header">
            <h3 class="form__header-label">Создать</h3>
        </div>
        <div class="form__inputs">
            <div class="form__input">
                <h4 class="form__input-label">Тип:</h4>
                <select @change='onChange' v-model='selected' class="form__input-field" name="type">
                    <option v-for='tp in KeysTypes' :value='tp.key' :key='tp.key'>{{tp.value.name}}</option>
                </select>
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Имя:</h4>
                <input v-model='name' class="form__input-field" type="text" name="name">
            </div>
            <div v-if='show[1]' class="form__input">
                <h4 class="form__input-label">Файл:</h4>
                <label class="form__input-field" for="uploadFile">{{fileLabel}}</label>
                <input @change='onUpload' hidden  id='uploadFile' type="file" name="password">
            </div>
            <div v-if='show[2]' class="form__input">
                <h4 class="form__input-label">Предодаватель:</h4>
                <select v-model='teacher' class="form__input-field" name="type">
                    <option v-for='teacher in getTeachers' :value='teacher.id' :key='teacher.id'>{{teacher.firstName}} {{teacher.lastName}}</option>
                </select>
            </div>
        </div>
        <div class="form__submit">
            <input :disabled='disabledSubmit' hidden class="form__input-field" type="submit" id="submit">
            <label for='submit' class="form__input-label">Создать {{getTypeName}}</label>
        </div>
    </form>
    <svg @click.prevent='closePopup' class='form__close' xmlns="http://www.w3.org/2000/svg" viewBox="0 0 320 512">
        <path fill='#B6B2B1' d="M310.6 150.6c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L160 210.7 54.6 105.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3L114.7 256 9.4 361.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0L160 301.3 265.4 406.6c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L205.3 256 310.6 150.6z"/>
    </svg>
    {{getTeachers +'asd'}}
    </div>
    
</template>

<script>
    import { mapGetters } from 'vuex'
    export default {
        data (){
            return{
                show: [false, false, false],
                selected: '',
                name: '',
                teacher: null,
                uploadedFile: null,
                
            }
        },
        
        inject: ['types'],
        methods: {
            closePopup(){
                this.$emit('close')
            },
            onSubmit(){
                this.$emit('create', this.selected, this.name, this.uploadedFile)
                
            },
            onUpload(evt){
                this.uploadedFile = evt.target.files[0]
            },
            onChange(evt){
                this.show =  [false, false, false]
                this.show[evt.target.selectedIndex] = true
                console.log()
            }
        },
        computed:{
            fileLabel(){
                return this.uploadedFile ? this.uploadedFile.name : 'Загрузить' 
            },
            getTypeName(){
                let obj = this.types.objects[this.selected]
                return  obj ? obj.onCreate_name : ''
            },
            disabledSubmit(){
                let nameExists = this.selected && this.name
                return !(this.selected == 'file' ? this.uploadedFile && nameExists : nameExists)
            },
            KeysTypes(){
                return this.types.KeysTypes.filter(tp => tp.value.permissions.create)
            },
            ...mapGetters(['getTeachers'])
        },
        
    }


</script>

<style scoped>
    label[for='uploadFile']{
        height: 26px;
        text-align: center;
        font-size: 14px;
        overflow: hidden;
        white-space: nowrap;
        position: relative;
        text-overflow: ellipsis;
    }
    label[for='uploadFile']::after{
        content: '';
        display: none;
        width: 100%;
        top: 0;
        left: 0;
        height: 100%;
        background-color: green;
        position: absolute;
    }
    @import url('@/assets/css/main/form.css');
    @import url('@/assets/css/main/popup.css');

</style>