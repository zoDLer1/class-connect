import { useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useRequest } from 'hooks/useRequest';
import UsersService from 'services/usersService';
import { GlobalUIContext } from 'contexts/GlobalUIContext';
import user from 'store/user';

const GroupEnter = () => {
    const { id } = useParams()
    const navigate = useNavigate()
    const { alert } = useContext(GlobalUIContext)
    const [send] = useRequest(
        async (id) => UsersService.group_enter(id),
        {
            403: () => {
                alert.show('Вы не можете вступить в группу')
               
            },
            400: (response) =>{
                alert.show(response.response.data.errorText)
                
            },
            200: () => {
                alert.show('Вы вступили в группу')
               
            }
        }
    )

    useEffect(()=>{
        const GroupEnter = async () =>{
            await send(id)
            navigate(`/files/${user.data.folder}`)
        }
        GroupEnter()

        
    }, [])

    return (
        <div>
            {id}
        </div>
    );
}

export default GroupEnter;
