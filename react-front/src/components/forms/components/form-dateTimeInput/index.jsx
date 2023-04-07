import Input from "../../../UI/Input"
import css from './form-dateTimeInput.module.css'
import useDateTime from "hooks/useDateTime"


function DateInput({ value, hidden=false, validation_methods }) {
    const { getProps } = useDateTime({value, validation_methods})
      return (
        <div className={css.block}>
            <Input {...getProps()} title='Срок сдачи:' type="date" onClick={(evt) => evt.target.showPicker()}/>
            <Input type="time" onClick={(evt) => evt.target.showPicker()}/>
        </div>

  )
}

export default DateInput
