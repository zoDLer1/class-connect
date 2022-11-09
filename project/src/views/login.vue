<template>
    <div class="content">
        <form @submit.prevent='onSubmit'  action="" class="form">
            <div class="form__header">
                <h3 class="form__header-label">Вход</h3>
            </div>
            <div class="form__inputs">
                <div class="form__input">
                    <h4 class="form__input-label">Почта:</h4>
                    <input v-model='email' class="form__input-field" type="text" name="email">
                </div>
                <div class="form__input">
                    <h4 class="form__input-label">Пароль:</h4>
                    <input v-model='password' class="form__input-field" type="password" name="password">
                </div>
            </div>
            <div class="form__group">  
                <a class="link password__fogotten" href="#">Забыли пароль?</a>
                <div class="form__checkbox">
                    <input hidden class="form__input-checkbox" type="checkbox" id="remember_me">
                    <label for="remember_me" class="form__input-label">Запомнить</label>
                </div>
            </div>

            <div class="form__submit">
                <input hidden class="form__input-field" type="submit" id="submit">
                <label for="submit" class="form__input-label">Войти</label>
            </div>
            <div class="form__link">
                Нет аккаунта? 
                <router-link class="link" :to="{name: 'register'}">Регистрация</router-link>
            </div>
        </form>
    </div>
</template>

<script>
    import { mapActions } from 'vuex'
    export default {
        name: 'login-view',
        data() {
            return {
                email: '',
                password: '',
            }
        },
        methods: {
            ...mapActions(['onLogin']),
            onSubmit(){
                this.onLogin({email: this.email, 
                password:this.password})
            },
            
        },
    }
</script>

<style scoped>
    @import url('@/assets/css/main/link.css');
    @import url('@/assets/css/main/form.css');
    .password__fogotten{
        font-size: 14px;
    }
</style>
