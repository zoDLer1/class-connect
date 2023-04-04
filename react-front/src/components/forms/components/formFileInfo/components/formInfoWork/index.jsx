
import FormInfoFolder from '../formInfoFolder';
import FormFileInfoWork from '../modules/formFileInfoWork';
import { faBriefcase } from '@fortawesome/free-solid-svg-icons';


const FormInfoWork = ({ id, setFilesInfo, ...workInfo }) => {


    

   

    const { creationTime, creatorName, ...workProps } = workInfo

    return <>
        <FormInfoFolder creationTime={creationTime} creatorName={creatorName} />
        <FormFileInfoWork title={'Работа'} setFilesInfo={setFilesInfo} {...workProps} id={id} icon={faBriefcase}/>
        
    </>
}


export default FormInfoWork