import DefaultApiInstanse from "api";

class UsersService{
    static async teachers(){
        return await DefaultApiInstanse.get('/User/teachers')
    }
    static async group_enter(id){
        return await DefaultApiInstanse.get("/group/enter", { params: {id} })
    }
    static async group_remove(id, studentId){
        return await DefaultApiInstanse.delete("/group/remove", { params: {id, studentId} })
    }
}
export default UsersService