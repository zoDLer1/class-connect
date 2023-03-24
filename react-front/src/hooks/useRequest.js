import { useContext, useState } from 'react'
import { GlobalUIContext } from 'contexts/GlobalUIContext'




export const useRequest = (func = async () => null, statuses={}) => {
    const { alert } = useContext(GlobalUIContext)

    const [waitingForResponse, setWaiting] = useState(false)     


    const handleResponse = (response, data) =>{
        console.log(response)
        const func = statuses[response.request.status]
        if (func) func(response, data)
    }

    const send = async (data) => {
        setWaiting(true)
        const response = await func(data).then(
            (success) => {
                handleResponse(success)
            },
            (error) => {
                if (error.code === 'ERR_NETWORK'){
                    alert.show('Сервер недоступен')     
                }
                handleResponse(error)
            }
        )
        setWaiting(false)
        return response
    }

    return [send, waitingForResponse]

}