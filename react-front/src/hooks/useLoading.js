import { useEffect, useState } from "react";

export function useLoading(func = async () => null, defaultLoad=true) {
    const [isLoading, setLoading] = useState(defaultLoad)

    const startLoading = () => setLoading(true)

    const stopLoading = () => setLoading(false)
    useEffect(()=>{
        const fetchLoading = async () =>{
            startLoading()
            await func()
            stopLoading()
       }
       fetchLoading()
       // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    return { isLoading, startLoading, stopLoading }

}
