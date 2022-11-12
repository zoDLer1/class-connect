<template>
    <div class="file-branch__item" @dblclick='getType(data.type.id-1).function(data.guid)'>
        <input class="file-branch__field" hidden name="file-branch" type="radio" :id="idComputed">
        <label :for="idComputed" class='file-branch__item-label'>
            <div class="file-branch__item-body">
                <div class="info__item-icon">
                    <img :src="getIconPath(data.type.id-1, data.mimeType)" alt="...">
                </div>
                <p class="file-branch__item-name">{{data.name}}</p> 
            </div>
            <svg class="file-branch__item-icon" width="8" height="14" viewBox="0 0 8 14" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M7.43788 7.7567C7.85292 7.33818 7.85292 6.65849 7.43788 6.23996L2.12538 0.882819C1.71034 0.464293 1.03632 0.464293 0.621277 0.882819C0.206238 1.30135 0.206238 1.98103 0.621277 2.39956L5.18339 7.00001L0.624597 11.6005C0.209558 12.019 0.209558 12.6987 0.624597 13.1172C1.03964 13.5357 1.71366 13.5357 2.1287 13.1172L7.4412 7.76005L7.43788 7.7567Z" fill="black" fill-opacity="0.2"/>
            </svg>  
        </label>
        
    </div>
</template>

<script>

    

export default {
    props:{
        data: Object,
        id: Number
    },
    methods:{
       

    },
    inject: ['getData', 'getType', 'getIconPath'],
    computed:{
        idComputed(){
            return `file-branch__item-${this.id}`
        },
   }
}
</script>