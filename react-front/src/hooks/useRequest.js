import { useContext, useState } from 'react'
import { AlertContext } from 'contexts/alertContext'




export const useRequest = (func = async () => null) => {
    const alertShow = useContext(AlertContext)
    const [waitingForResponse, setWaiting] = useState(false)     

    const send = async (data, statuses) => {
        setWaiting(true)
        const response = await func(data).then(
            (success) => {
                const func = statuses[success.request.status]
                if (func) func(success) 
            },
            (error) => {
                if (error.code === 'ERR_NETWORK'){
                    alertShow('Сервер недоступен')     
                }
                const func = statuses[error.request.status]
                if (func) func(error)
            }
        )
        setWaiting(false)
        return response
    }

    return { send, waitingForResponse }

}