<template>
    <div :style='setCoords' v-if='menu.show' class="files__branch-menu menu">
        <menu-item v-for='(obj, index) in menu.objs' :key='index' :object='obj'></menu-item>        
    </div>
</template>

<script>    
    import item from '@/components/menu/menu-item.vue'
    export default {
        props: {
            menu: Object
        },
        components:{
            'menu-item': item
        },
        computed:{
            setCoords(){
                let [x,y] = this.menu.coords
                let [x_offset,y_offset] = this.menu.offset
                return `top: ${y+y_offset}px; left: ${x+x_offset}px`
            },
        }
    }
</script>


<style scoped>
    @import url('@/assets/css/main/menu.css');
</style>