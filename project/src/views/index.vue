<template>
    <div class="content">
        <header class="header">
            <div class="header__nav">
                <div class="header__nav-link">
                    <!-- if user.is_authorized -->
                        <a href="#">Моя группа</a>
                </div>

                <div class="header__nav-link">
                    <!-- if !user.is_authorized -->
                        <router-link :to="{name: 'register'}">Регистрация</router-link>
                        |
                        <router-link :to="{name: 'login'}" href="/login.html">Войти</router-link>
                    <!-- else  -->
                        <!-- <a href="#">{{user.name}}</a> -->
                </div>
            </div>
        </header>
        <div class="info-block">
            <div class="info-block__body">
                <img src="/imgs/logo.png" alt="" class="info-block__logo">
                <svg width="5" height="138" viewBox="0 0 5 138" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M2.5 0.25V137.75" stroke="white" stroke-width="5"/>
                </svg>
                <h1 class="info-block__info">
                    Национальный<br>
                    исследовательский<br>
                    ядерный университет «МИФИ»
                </h1>
            </div>
            

            <div class="info-block__links">
                <a href="#" class="info-block__link">История кафедры</a>
            </div>

        </div> 
    </div>
    
</template>

<script>
    export default {
        name: 'index-view'
        
    }

</script>

<style scoped>
    @import "@/assets/css/main/header.css";
    @import "@/assets/css/main/header.css";


    .content{
        background-image: url('@/assets/imgs/image.png');
        background-repeat: no-repeat;
        background-size: cover;
        background-position: 80% 20%;
        background-attachment: fixed;
        display: block;
    }

    .info-block{
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    margin-top: 200px;
    }

    .info-block__body{
        display: flex;
        gap: 40px;
        align-items: center;
        background-color: rgba(0, 0, 0, 0.4);
        padding: 40px;
        margin-bottom: 10px;
    }
    .info-block__info{
        color: #fff;
    }
    .info-block__link{
        font-size: 20px;  
        font-weight: 500; 
    }

</style>