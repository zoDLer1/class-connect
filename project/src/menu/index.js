class MenuItem{
    constructor(name, icon, func) {
        this.name = name 
        this.icon = icon 
        this.func = func 
    }
} 

export default class Menu {
    coords = [0, 0]
    visible = false
    offset = [0, 0]
    index = null
    selected = null
    objs = []
    box_container = null

    constructor(objets = [], selector=null){
        for(let obj of objets){
            let item = new MenuItem(obj.name, obj.icon, obj.function)  
            this.objs.push(item)
        }
        this.box_container = selector
        // for (let elem of document.querySelectorAll(selector)){
        //     elem.onclick = this.toggle
        // }
    }
    get getCoords(){
        let [x,y] = this.coords
        let [x_offset,y_offset] = this.offset
        return `top: ${y+y_offset}px; left: ${x+x_offset}px`
    }
    show(evt, props={}){
        if (!this.box_container || evt.target.classList.contains(this.box_container)){
            this.visible = true 
            this.coords = [evt.pageX, evt.pageY]
            
            if (props){
                this.index = props.index
                this.selected = props.selected
            }
        }
        
    }
    hide(){
        this.visible = false
    }
    toggle(evt, props={}){
        this.visible ? this.hide() : this.show(evt, props)
    }
}

