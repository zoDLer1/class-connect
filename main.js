let folders = document.querySelectorAll('.folder__main')
for (let folder of folders) {
    folder.onclick = (evt) =>{
        let fol = folder.closest('.folder')
        
        if (fol.classList.contains('folder--selected')){
            fol.classList.remove('folder--selected')
        }
        else{
            fol.classList.add('folder--selected')
        }
    
    }
}
