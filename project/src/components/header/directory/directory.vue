<template>
    <a href="#" class="directories__directory">
        <h3 class="directories__text">{{info}}</h3>
    </a>
</template>



<script>
    export default{
        name: 'directory-component',
        props:{
            info: String,
        }
    }
</script>