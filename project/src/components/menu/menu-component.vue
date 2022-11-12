<template>
    <div v-if='menu.visible' @contextmenu.prevent='menu.hide' @click.prevent='menu.hide' class='menu'>
        <div :style='menu.getCoords'  class="menu__layout">
            <div class="menu__items">
                <menu-item v-for='obj in menu.objs' :key='obj.name' :object='obj'></menu-item> 
            </div>
            
        </div>
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
    }
</script>


<style scoped>
    @import url('@/assets/css/main/menu.css');
</style>