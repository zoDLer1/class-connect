import pagesCss from '../pages.module.css'
import FilesForm from 'components/forms/files-form'
import user from 'store/user'
import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'


function Files(props) {
    const navigate = useNavigate()
    useEffect(() => {
        /* eslint-disable react-hooks/exhaustive-deps */
        if (user.data === null) {
            navigate('/login')
        }
    }, [])

    return (
        <div {...props} className={[pagesCss.default_background, pagesCss.content_position_center].join(' ')}>
            {user.data ?
                <FilesForm />
                : null
            }
        </div>
    )
}

export default Files
