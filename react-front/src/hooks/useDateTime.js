import useValidateInput from './useValidateInput'
import dateFormat from 'dateformat'

function useDateTime({ value, validation_methods }) {



    const { onChanged } = useValidateInput({ value, ...validation_methods })
    

    const getTime = () => ({
        value: value ? dateFormat(value, 'HH:MM') : '',
        onChange: (evt) => {
            const [hours, minutes] = evt.target.value.split(':')
            console.log(hours, minutes)
            const updatedTime = value ? new Date(value) : new Date()
            updatedTime.setHours(hours)
            updatedTime.setMinutes(minutes)
            onChanged(updatedTime)
        }
    })

    const getDate = () => ({
        value: value ? dateFormat(value, 'yyyy-mm-dd') : '',
        onChange: (evt) => 
        {
            const [year, month, day] = evt.target.value.split("-")
            console.log(year, month, day)
            let updatedTime = new Date(value)
            
            if (!value){
                updatedTime = new Date(year, month-1, day, 1, 0)
            }
            else{
                updatedTime.setFullYear(year)
                updatedTime.setMonth(Number(month)-1)
                updatedTime.setDate(Number(day))
            }
            

            return onChanged(updatedTime)
        }
        
    })


    return { getTime, getDate }
}

export default useDateTime
