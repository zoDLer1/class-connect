<template>
    <div class="content">
        <form action="" class="form">
        <div class="form__header">
            <h3 class="form__header-label">Регистрация</h3>
        </div>
        <div class="form__inputs">
            <div class="form__input">
                <h4 class="form__input-label">Имя:</h4>
                <input class="form__input-field" type="text" name="firstname">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Фамилия:</h4>
                <input class="form__input-field" type="text" name="lastname">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Почта:</h4>
                <input class="form__input-field" type="text" name="email">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Пароль:</h4>
                <input class="form__input-field" type="password" name="password">
            </div>
            <div class="form__input">
                <h4 class="form__input-label">Повторите <br> пароль:</h4>
                <input class="form__input-field" type="password" name="repeat_password">
            </div>
            
        </div>
        <div class="form__submit">
            <label class="form__input-label">Создать аккаунт</label>
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
    export default {
        name: 'register-view'
    }
</script>

<style scoped>
    @import url('@/assets/css/main/form.css');
    @import url('@/assets/css/main/link.css');
</style>