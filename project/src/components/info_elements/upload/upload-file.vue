<template>
    <div class="info__upload-item">

        <div class="info__upload-label">{{data.name}}</div>
        <svg @click='DeleteFile(id)' width="15" height="15" viewBox="0 0 19 20" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M18.4492 3.82422C19.1816 3.0918 19.1816 1.90234 18.4492 1.16992C17.7168 0.4375 16.5273 0.4375 15.7949 1.16992L9.625 7.3457L3.44922 1.17578C2.7168 0.443359 1.52734 0.443359 0.794922 1.17578C0.0625 1.9082 0.0625 3.09766 0.794922 3.83008L6.9707 10L0.800781 16.1758C0.0683593 16.9082 0.0683593 18.0977 0.800781 18.8301C1.5332 19.5625 2.72266 19.5625 3.45508 18.8301L9.625 12.6543L15.8008 18.8242C16.5332 19.5566 17.7227 19.5566 18.4551 18.8242C19.1875 18.0918 19.1875 16.9023 18.4551 16.1699L12.2793 10L18.4492 3.82422Z" fill="black"/>
        </svg>
        
    </div>
</template>

<script>
    export default {
        inject: ['DeleteFile'],
        props:{
            data: Object,
            id: Number,
        },
        methods:{
            preview(){  
                let reader = new FileReader();
                reader.readAsText(this.data);
                reader.onload = function() {
                    console.log(reader.result);
                };
            }
        },

    }
</script>