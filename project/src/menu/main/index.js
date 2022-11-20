import { Menu }from '@/menu' 
// import types from '@/types/subjects'



export class MainMenu extends Menu{}


export class ItemMenu extends Menu{
    

    toggle(evt, props={}){
        this.visible ? this.hide() : this.show(evt, props)
    }
    show(evt, props={}){
        super.show(evt)
        if (props){
            this.index = props.index
            this.selected = props.selected
        } 
    }
}



// let item = new MenuItem(obj.name, obj.icon, obj.function, obj.condition)  




