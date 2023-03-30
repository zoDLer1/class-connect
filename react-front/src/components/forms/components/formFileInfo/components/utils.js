export const parse_time = (string_time) =>{
    const [date, exactTime] = string_time.split('T')
    const [time] =  exactTime.split('.')
    return [date, time].join(' ')
}