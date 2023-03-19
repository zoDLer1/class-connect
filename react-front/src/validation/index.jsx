export const MAX_LENGTH = (length, errorMessage = `Максимальная длина ${length} символов`) => {
    return (value) => value.length > length ? errorMessage : ''
}

export const IS_EMAIL = (errorMessage = 'Введите корректный email') => {
    return (value) =>
        !value.match(/^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/)
            ? errorMessage
            : ''

}
export const REQUIRED = (errorMessage = 'Поле не должно быть пустым') => {
    return (value) => !value ? errorMessage : ''
}

export const MIN_LENGTH = (length, errorMessage = `Минимальная длина ${length} символов`) =>{
    return (value) => value.length < length ? errorMessage : ''
}