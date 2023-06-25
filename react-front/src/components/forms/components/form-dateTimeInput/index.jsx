import Input from "../../../UI/Input"
import css from './form-dateTimeInput.module.css'
import useDateTime from "hooks/useDateTime"
import { useState, useEffect } from 'react';

function DateInput({ value, error, hidden = false, validation_methods }) {
    const [localHidden, setLocalHidden] = useState(hidden)

    useEffect(() => {
        setLocalHidden(hidden)
    }, [hidden])

    const { getDate, getTime } = useDateTime({ value, validation_methods })
    if (!localHidden) {

        return (
            <div className={css.block}>
                <div className={css.body}>
                    <Input {...getDate()} error={error} title='Срок сдачи:' type="date" onClick={(evt) => evt.target.showPicker()} />
                    <Input {...getTime()} type="time" onClick={(evt) => evt.target.showPicker()} />
                </div>
            </div>

        )
    }
}

export default DateInput
