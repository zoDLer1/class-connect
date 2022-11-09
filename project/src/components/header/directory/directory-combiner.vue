<template>
    <a @click='toggle' href="#" class="directories__directory">
        <h3 class="directories__text">{{info}}</h3>
        
    </a>
    <div class="directories__combiner">
        <div v-if='show' class="directories__combiner-items">
            <div v-for='item in items' :key='item' @click="getData(item.guid)" class="directories__combiner-item">
                <svg class="directories__combiner-icon" width="31" height="27" viewBox="0 0 31 27" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M4.25 26.625H26.75C28.8184 26.625 30.5 24.9434 30.5 22.875V7.875C30.5 5.80664 28.8184 4.125 26.75 4.125H17.9902C16.9941 4.125 16.0391 3.73242 15.3359 3.0293L13.7832 1.4707C13.0801 0.767578 12.125 0.375 11.1289 0.375H4.25C2.18164 0.375 0.5 2.05664 0.5 4.125V22.875C0.5 24.9434 2.18164 26.625 4.25 26.625Z" fill="#F4B43F"/>
                </svg>
                <p class="directories__combiner-text">{{item.name}}</p>  
            </div>
        </div>
    </div>
</template>

<script>
    export default{
        name: 'directory-component',
        props:{
            info: String,
            items: Array
        },
        data(){
            return{
                show: false
            }
        },
        inject: ['getData'],
        methods:{
            toggle(evt){
                evt.preventDefault();
                this.show = !this.show
            }
        }
    }
</script>