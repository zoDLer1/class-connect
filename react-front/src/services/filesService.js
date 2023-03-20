import DefaultApiInstanse from "api";

class FilesService {

    static async get_folder(id) {
        return await DefaultApiInstanse.get('/FileSystem', {params: {id: id}})
    }
    static async remove(id){
        return await DefaultApiInstanse.delete('/FileSystem', {params: {id: id}})
    }

}
export default FilesService