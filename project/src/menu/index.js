class MenuItem{
    constructor({name, icon, func, condition}) {
        this.name = name 
        this.path = icon 
        this.func = func 
        this.condition = condition === undefined ? true : condition
    }
    get icon(){
        return require(`#/${this.path}`)
    }
    setCondition(cond){
        this.condition = cond
    }
} 

export const Menu = class Menu{
    coords = [0, 0]
    visible = false
    offset = [0, 0]
    box_container = null
    objs = {}
    
    constructor(objects, selector=null){
        for (let obj in objects){
            let item = new MenuItem(objects[obj])
            this.objs[obj] = item
        }
        this.box_container = selector
    }

    get getCoords(){
        let [x,y] = this.coords
        let [x_offset,y_offset] = this.offset
        return `top: ${y+y_offset}px; left: ${x+x_offset}px`
    }
    show(evt){
        if (!this.box_container || evt.target.classList.contains(this.box_container)){
            this.visible = true 
            this.coords = [evt.pageX, evt.pageY]
        }
        
    }
    hide(){
        this.visible = false
    }
    toggle(evt){
        this.visible ? this.hide() : this.show(evt)
    }
}

