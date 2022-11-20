<template>
    <div class="content">
        <form @submit.prevent='onSumbit' action="" class="form">
        <div class="form__header">
            <h3 class="form__header-label">Регистрация</h3>
        </div>
        <div class="form__inputs">
            <div class="form__input">
                <h4 class="form__input-label">Имя:</h4>
                <input v-model='firstName' class="form__input-field" type="text" name="firstname">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Фамилия:</h4>
                <input v-model="lastName" class="form__input-field" type="text" name="lastname">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Отчество:</h4>
                <input v-model='patronymic' class="form__input-field" type="text" name="patronymic">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Почта:</h4>
                <input v-model='email' class="form__input-field" type="text" name="email">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Пароль:</h4>
                <input v-model='password' class="form__input-field" type="password" name="password">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Повторите <br> пароль:</h4>
                <input v-model='repeat_password' class="form__input-field" type="password" name="repeat_password">
            </div>
            
        </div>
        <div class="form__submit">
            <label for='submit' class="form__input-label">Создать аккаунт</label>
            <input hidden class="form__input-field" type="submit" id="submit">
        </div>
        <div class="form__link">
            Уже есть аккаунт? 
            <router-link class="link" :to="{name: 'login'}">Войдите</router-link>
        </div>
    </form>
    </div>
    
</template>

<script>
    import { mapActions} from 'vuex'
    export default {
        name: 'register-view',
        data(){
            return {
                firstName: '',
                lastName: '',
                patronymic: '',
                email: '',
                password: '',
                
                repeat_password: '',

            }
        },
        methods:{
            onSumbit(){
                let data = {
                    firstName: this.firstName,
                    lastName: this.lastName,
                    patronymic: this.patronymic,
                    password: this.password,
                    email: this.email
                }
                if (this.repeat_password == this.password && this.password)
                    this.onRegister(data).then(()=>{
                        console.log('2')
                        this.$router.push({name: 'subjects'})
                    })
            },
            ...mapActions(['onRegister'])
        }
    }
</script>

<style scoped>
    @import url('@/assets/css/main/form.css');
    @import url('@/assets/css/main/link.css');
</style>