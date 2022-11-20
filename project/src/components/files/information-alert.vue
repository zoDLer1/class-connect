<template>
    <div @animationend='animEnd' v-if='object.show' class="alert">
        {{object.info}}
    </div>
</template>

<script>
    export default{
        name: 'information-alert',
        props:{
            object: Object
        },
        methods:{
            animEnd(evt){
                if (evt.animationName == 'opening'){
                    this.object.toggle()
                } 
            }
        }
    }
</script>

<style scoped>
    @import url('@/assets/css/main/alert.css');
</style>