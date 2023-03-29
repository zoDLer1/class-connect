import DefaultApiInstanse from "api";

class FilesService {

    static async get_folder(id, responseType='json') {
        return await DefaultApiInstanse.get('/FileSystem', { params: { id: id }, responseType: responseType })
    }
    static async remove(id) {
        return await DefaultApiInstanse.delete('/FileSystem', { params: { id: id } })
    }
    static async create(id, { name, type, teacherId, uploadedFiles }) {
        console.log(uploadedFiles)
        if (uploadedFiles?.length){
            const formData = new FormData()
            formData.append('id', id)
            formData.append('uploadedFile', uploadedFiles[0])
            return await DefaultApiInstanse.post('/FileSystem/file', formData)
        }
        return await DefaultApiInstanse.post('/FileSystem', { id, name, type, teacherId })
    }
    static async rename(id, name){
        return await DefaultApiInstanse.patch('/FileSystem', null, { params: { id: id, name }})
    }

}
export default FilesService