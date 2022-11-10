<template>
    <div class="content">
        <div class="page__section">
            <inner-header :username="user" :realPath='getRealPath' :path="getPath"></inner-header>
            <div class="files">
                <file-branch :data='getItems'></file-branch>
                <div class="file__info info">
                    <div class="info__header">
                        <h3 class="info__header-label">Предмет 2</h3>
                        <!-- <p class="info__header-sub">до 23 ноября</p> -->
                    </div>
                    <div class="info__item">
                        <input class="info__item-field" hidden type="checkbox" id="info">
                        <label for="info" class="info__item-header">
                            <div class="info__item-header-body">
                                <svg class="info__item-icon" width="28" height="28" viewBox="0 0 28 28" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M14 28C21.7328 28 28 21.7328 28 14C28 6.26719 21.7328 0 14 0C6.26719 0 0 6.26719 0 14C0 21.7328 6.26719 28 14 28ZM11.8125 18.375H13.125V14.875H11.8125C11.0852 14.875 10.5 14.2898 10.5 13.5625C10.5 12.8352 11.0852 12.25 11.8125 12.25H14.4375C15.1648 12.25 15.75 12.8352 15.75 13.5625V18.375H16.1875C16.9148 18.375 17.5 18.9602 17.5 19.6875C17.5 20.4148 16.9148 21 16.1875 21H11.8125C11.0852 21 10.5 20.4148 10.5 19.6875C10.5 18.9602 11.0852 18.375 11.8125 18.375ZM14 10.5C13.032 10.5 12.25 9.71797 12.25 8.75C12.25 7.78203 13.032 7 14 7C14.968 7 15.75 7.78203 15.75 8.75C15.75 9.71797 14.968 10.5 14 10.5Z" fill="#0057B8"/>
                                </svg>
                                <h2 class="info__item-label">Информация</h2>
                            </div>
                            <svg class="info__item-icon" width="13" height="9" viewBox="0 0 13 9" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M5.7433 7.93794C6.16182 8.35298 6.84151 8.35298 7.26004 7.93794L12.6172 2.62544C13.0357 2.2104 13.0357 1.53638 12.6172 1.12134C12.1987 0.706299 11.519 0.706299 11.1004 1.12134L6.49999 5.68345L1.89954 1.12466C1.48102 0.709619 0.801331 0.709619 0.382804 1.12466C-0.035723 1.5397 -0.035723 2.21372 0.382804 2.62876L5.73995 7.94126L5.7433 7.93794Z" fill="black"/>
                            </svg>
                        </label>
                        <div class="info__components">
                            <div class="info__component">
                                <div class="info__component-body">
                                    <div class="info__component-body-wrapper">
                                        <div class="info__component-icon">
                                            <svg width="26" height="30" viewBox="0 0 26 30" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                <path d="M12.911 15C16.8563 15 20.0539 11.7545 20.0539 7.75C20.0539 3.74551 16.8563 0.5 12.911 0.5C8.96568 0.5 5.76814 3.74551 5.76814 7.75C5.76814 11.7545 8.96568 15 12.911 15ZM10.3608 17.7188C4.86412 17.7188 0.410995 22.2387 0.410995 27.8178C0.410995 28.7467 1.15318 29.5 2.06836 29.5H23.7536C24.6688 29.5 25.411 28.7467 25.411 27.8178C25.411 22.2387 20.9579 17.7188 15.4612 17.7188H10.3608Z" fill="#0057B8"/>
                                            </svg>
                                        </div>
                                        <p class="info__component-label">Преподаватель</p>
                                    </div>
                                    
                                </div>
                                <p class="info__component-value">Купцов Валентин Анатольевич</p>
                            </div>
                            <div class="info__component">
                                <div class="info__component-body">
                                    <div class="info__component-body-wrapper">
                                        <div class="info__component-icon">
                                            <svg width="23" height="29" viewBox="0 0 23 29" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                <path d="M11.5 0.284302C9.17884 0.284302 7.20197 1.76695 6.47453 3.83822H3.50368C2.03213 3.83822 0.838234 5.03212 0.838234 6.50366V26.0502C0.838234 27.5218 2.03213 28.7157 3.50368 28.7157H19.4963C20.9679 28.7157 22.1618 27.5218 22.1618 26.0502V6.50366C22.1618 5.03212 20.9679 3.83822 19.4963 3.83822H16.5255C15.798 1.76695 13.8212 0.284302 11.5 0.284302ZM11.5 7.39214C10.5171 7.39214 9.72304 6.59807 9.72304 5.61518C9.72304 4.6323 10.5171 3.83822 11.5 3.83822C12.4829 3.83822 13.277 4.6323 13.277 5.61518C13.277 6.59807 12.4829 7.39214 11.5 7.39214ZM7.0576 10.9461H15.9424C16.4311 10.9461 16.8309 11.3459 16.8309 11.8345C16.8309 12.3232 16.4311 12.723 15.9424 12.723H7.0576C6.56893 12.723 6.16912 12.3232 6.16912 11.8345C6.16912 11.3459 6.56893 10.9461 7.0576 10.9461Z" fill="#0057B8"/>
                                            </svg>
                                        </div>
                                        <p class="info__component-label">Описание</p>
                                    </div>
                                    
                                </div>
                                <p class="info__component-value">Lorem ipsum dolor sit amet, consectetur adipisicing elit. Iusto aliquam placeat deleniti quod officia accusamus omnis. Eius nihil odio eligendi similique voluptatem quibusdam, ut aspernatur dignissimos repellendus porro. Nemo accusamus tempore libero quas necessitatibus possimus ipsam illum, exercitationem a ipsa neque? Repellendus culpa, dignissimos sapiente eveniet recusandae, neque voluptatum aliquid minus illo iure maxime omnis non ut! Placeat laborum rerum temporibus, et deleniti dolorum magni voluptates suscipit, aliquid consectetur a, consequatur tempore odio pariatur. Possimus vero harum debitis et at obcaecati itaque unde. Aut accusamus natus accusantium alias veniam sed omnis, id enim maxime, pariatur inventore. Error quia fugit nemo vitae natus atque velit minima perspiciatis necessitatibus voluptate reiciendis mollitia nam doloremque harum maiores enim dicta iusto, aut magnam veritatis, vel sint? Quos!</p>
                            </div>
                        </div>
                    </div>
                    <upload-element></upload-element>
                    
                </div>
            </div>
        </div>
    </div>
</template>


<style scoped>
    @import url('@/assets/css/main/file-branch.css');
    @import url('@/assets/css/main/info.css');
    @import url('@/assets/css/main/files.css');
</style>

<script>
    import {API} from '@/api/requests'
    import upload from '@/components/info_elements/upload-element.vue'
    import innerHeader from '@/components/header/inner-header.vue'
    import filebranch from '@/components/files/file-branch.vue'
    import { mapActions, mapGetters } from 'vuex'
    
    export default {
        computed:{
            ...mapGetters(['getItems', 'getGuid', 'getPath', 'getRealPath'])
        },
        name: 'subject-view',
        components:{
            'file-branch': filebranch,
            'inner-header': innerHeader,
            'upload-element': upload,
        },
        data() {
            return {
                user: 'zoDLer',

                api: API,

                types: {
                    items:[
                        {function: this.getData, mimeTypes: {null:'folder.svg'}},
                        {function: (_)=> _,  mimeTypes: {
                                                'text/plain': 'file.svg',
                                                'image/jpeg': 'image.svg',
                                                'image/png': 'image.svg'
                                                }
                        },
                        {function: this.getData,  mimeTypes: {null:'group.svg'}},
                        {function: this.getData,  mimeTypes: {null:'folder.svg'}},
                    ],
                    default:{
                        mimeType: 'file.svg'
                    }
                }
                    
            }
        },
        provide() {
            return {
                getData: this.getData,
                getType: this.getType,
                getIconPath: this.getIconPath,
                requireIcon: this.requireIcon,
            }
        },
        methods: {
            ...mapActions(['getData', 'updateData']),
            requireIcon(icon){
                return require(`#/${icon}`)
            },

           

            getType(type){
                return this.types.items[type]
            },
            getIconPath(type, mimeType){
                type = this.getType(type)
                let src = type.mimeTypes[mimeType]
                return this.requireIcon(src ? src : this.types.default.mimeType)
            },
        },
        
        created() {    
            this.getData('87c229ab-b23d-4c5c-bb23-583b1fbc4aa1')
        },
        
        
    }    



</script>