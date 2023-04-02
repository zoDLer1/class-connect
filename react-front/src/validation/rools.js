export const MAX_LENGTH_ROOL = (length) => (value) => value.length <= length

export const IS_NUMERIC_ROOL = () => (value) => /^-?\d+$/.test(value) || !value
    
export const NUMER_BETWEEN_ROOL = (min, max) => (value) => {
    const number = Number(value)
    return (min <= number && number <= max) || !value
}
    