import css from './form-select.module.css'
import useInput from 'hooks/useInput';
import { useRequest } from 'hooks/useRequest';
import { useState, useEffect } from 'react';
import FormInputLoader from '../form-input-loader';

const FormSelect = ({ title, error, options, value, hidden = false, validation_methods }) => {



    const [dataIsLoaded, setLoaded] = useState(false)
    const [loadData, isLoading] = useRequest(options.asyncLoadOptions,
        {
            200: (response) => {
                const data = options.mapping ? response.data.map(options.mapping) : response.data
                setSelfOptions(data)
                if (data.length) {
                    validation_methods.setValue(data[0].id)
                }
                setLoaded(true)
            }
        }
    )

    useEffect(() => {
        if (options.asyncLoadOptions && !dataIsLoaded && !hidden) {
            loadData()
        }
    }, [hidden])

    const [selectOptions, setSelfOptions] = useState(options.values || [])

    const [localHidden, setLocalHidden] = useState(hidden)

    useEffect(() => {
        setLocalHidden(hidden)
    }, [hidden])

    const { getProps } = useInput({ value, validation_methods })

    return (
        <>
            {!localHidden &&
                <div className={css.block}>

                    {title && <h4 className={css.title}>{title}</h4>}
                    <div className={css.body}>
                        <div className={css.select}>
                            <select className={css.input} {...getProps()}>
                                {selectOptions.map(item => <option value={item.id} key={item.id}>{item.text}</option>)}
                            </select>
                            {isLoading &&
                                <div className={css.loader}>
                                    <FormInputLoader />
                                </div>
                            }
                        </div>



                        <span className={css.error}>{error}</span>
                    </div>
                </div>
            }
        </>
    );

}

export default FormSelect;
