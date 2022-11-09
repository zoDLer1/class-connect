<template>
<div class="info__item">
    <input class="info__item-field" checked hidden type="checkbox" id="upload">
    <label for="upload" class="info__item-header">
        <div class="info__item-header-body">
            <svg class="info__item-icon" width="28" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">
                <path d="M184 48H328c4.4 0 8 3.6 8 8V96H176V56c0-4.4 3.6-8 8-8zm-56 8V96H64C28.7 96 0 124.7 0 160v96H192 320 512V160c0-35.3-28.7-64-64-64H384V56c0-30.9-25.1-56-56-56H184c-30.9 0-56 25.1-56 56zM512 288H320v32c0 17.7-14.3 32-32 32H224c-17.7 0-32-14.3-32-32V288H0V416c0 35.3 28.7 64 64 64H448c35.3 0 64-28.7 64-64V288z"/>
            </svg>
            <h2 class="info__item-label">Работа</h2>
        </div>
        <svg class="info__item-icon" width="13" height="9" viewBox="0 0 13 9" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M5.7433 7.93794C6.16182 8.35298 6.84151 8.35298 7.26004 7.93794L12.6172 2.62544C13.0357 2.2104 13.0357 1.53638 12.6172 1.12134C12.1987 0.706299 11.519 0.706299 11.1004 1.12134L6.49999 5.68345L1.89954 1.12466C1.48102 0.709619 0.801331 0.709619 0.382804 1.12466C-0.035723 1.5397 -0.035723 2.21372 0.382804 2.62876L5.73995 7.94126L5.7433 7.93794Z" fill="black"/>
        </svg>
    </label>
    <div class="info__components">

        <div class="info__upload">
            <div class="info__upload-body">
                <upload-file v-for='(file, index) in files' :key='index' :id='index' :data='file'></upload-file>
            </div>
            <form action="" class="info__upload-btns">
                <div class="info__button info__upload-add_file">
                    <input @change='Upload' multiple hidden accept=".png" type="file" id="add_file">
                    <label for="add_file" class="info__button-body">
                        <svg width="22" height="21" viewBox="0 0 22 21" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M12.5625 1.90625C12.5625 1.04199 11.8643 0.34375 11 0.34375C10.1357 0.34375 9.4375 1.04199 9.4375 1.90625V8.9375H2.40625C1.54199 8.9375 0.84375 9.63574 0.84375 10.5C0.84375 11.3643 1.54199 12.0625 2.40625 12.0625H9.4375V19.0938C9.4375 19.958 10.1357 20.6562 11 20.6562C11.8643 20.6562 12.5625 19.958 12.5625 19.0938V12.0625H19.5938C20.458 12.0625 21.1562 11.3643 21.1562 10.5C21.1562 9.63574 20.458 8.9375 19.5938 8.9375H12.5625V1.90625Z" fill="#0057B8"/>
                        </svg>
                        Добавить файл
                    </label>
                    
                    
                    
                </div>
                <div class="info__button info__upload-submit_work">

                    <label for="submit_work" class="info__button-body">
                        Сдать работу
                    </label>
                    <input hidden type="submit" id="submit_work">
                    
                </div>
            </form>
        </div>
    </div>
</div>
</template>

<script>

    import file from '@/components/info_elements/upload/upload-file.vue'
    export default {
        provide(){
            return{
                DeleteFile: this.DeleteFile
            }
        },
        data(){
            return{
                files: []
            }
        },
        components:{
            'upload-file': file
        },
        methods:{
            Upload(evt){
                this.files = Array.from(evt.target.files)
                
            },
            DeleteFile(index){
                console.log(this.files.splice(index, 1))
            },
        }

    }

</script>