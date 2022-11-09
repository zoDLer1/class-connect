<template>
    <div @click='object.function' class="menu__item">
    <img :src="object.icon" alt="">
        <p class="menu__item-text">{{object.name}}</p>
    </div>
</template>

<script>
    export default{
        props:{
            object: Object
        }
    }
</script>