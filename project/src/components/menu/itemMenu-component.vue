<template>
    <div v-if='menu.visible && filtered.length' @contextmenu.prevent='menu.hide' @click.prevent='menu.hide' class='menu'>
        <div :style='menu.getCoords'  class="menu__layout">
            <div class="menu__items">
                <menu-item v-for='obj in filtered' :key='obj.name' :object='obj'></menu-item> 
            </div>
            
        </div>
    </div>
</template>

<script>
    import item from '@/components/menu/menu-item.vue'
    export default{
        components:{'menu-item':item},
        inject: ['types'],
        props: {
            menu: Object
        },
        computed:{
            filtered(){
                let list = []
                    for (let obj in this.menu.objs){
                        let permission = this.types.byTypeIndex(this.menu.selected.type.id-1).permissions[obj] 
                        let condition = this.menu.objs[obj].condition

                        if (permission !== undefined){
                            if (permission && condition){
                                list.push(this.menu.objs[obj])
                            }
                        }
                        else if (condition){
                            list.push(this.menu.objs[obj])
                        }
                    }
                return list
                
            },
            view(){
                console.log()
                return this.menu.visible
            }
        }
    }

</script>