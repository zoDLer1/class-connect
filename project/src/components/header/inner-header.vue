<template>
    <header class="inner-header">
        <div class="inner-header__directories directories">
            <div v-for='part in separatedPath()' :key='part' style='display: flex'>
                <directory-component v-if='part.type == "part"' :info='part.name'  @click="getData(part.guid)"></directory-component>
                <directoty-combiner v-else :info='part.name' :items='part.items'></directoty-combiner>
                <directoty-seporator v-if='part.appendSeporator'></directoty-seporator>
            </div>
        </div>
        <a @click.prevent='logout' class="inner-header__user" href="#">{{getUser}}</a>
        
    </header>
    
</template>


<script>
    import directory from '@/components/header/directory/directory.vue'
    import seporator from '@/components/header/directory/directoty-seporator.vue'
    import combiner from '@/components/header/directory/directory-combiner.vue'
    import { mapActions, mapGetters } from 'vuex'
    export default{
        components:{
            'directory-component': directory,
            'directoty-seporator': seporator,
            'directoty-combiner':combiner,
        },
        props: {
            username: String,
            path: Array,
            realPath: Array
        },
        inject: ['getData'],
        methods: {
            ...mapActions(['onLogout']),
            logout(){
                this.onLogout()
                this.$router.push({name:'login'})
            },
            separatedPath(){
                let path = []
                let realPath = this.realPath
                let splitedPath = this.path
                for (const [index, part] of splitedPath.entries()){
                    path.push({type: 'part', guid:realPath[index], name: part, appendSeporator: index+1 != splitedPath.length})   
                }
                if (path.length > 3){
                    path.splice(1,0,{type: 'combiner', name:'...', appendSeporator: true, items: path.splice(1, path.length-3)})
                }
                return path
            },
        },
        computed: mapGetters(['getUser'])
    }

</script>

<style>
    @import url('@/assets/css/main/inner-header.css');
    @import url('@/assets/css/main/directories.css');
</style>

