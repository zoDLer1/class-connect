import DefaultApiInstanse from "api";

class FilesService {

    static async get_folder(id) {
        return await DefaultApiInstanse.get('/FileSystem', { params: { id: id } })
    }
    static async remove(id) {
        return await DefaultApiInstanse.delete('/FileSystem', { params: { id: id } })
    }
    static async create(id, { name, type, teacherId, uploadedFile }) {
        return await DefaultApiInstanse.post('/FileSystem', { id, name, type, teacherId })
    }
    static async rename(id, name){
        return await DefaultApiInstanse.patch('/FileSystem', null, { params: { id: id, name }})
    }

}
export default FilesService