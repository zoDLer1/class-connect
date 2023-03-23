import DefaultApiInstanse from "api";

class UsersService{
    static async teachers(){
        return await DefaultApiInstanse.get('/User/teachers')
    }
}
export default UsersService