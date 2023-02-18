import pagesCss from '../pages.module.css'
import FilesForm from 'components/forms/files-form'

function Files( props ) {
    return (
        <div {...props} className={[pagesCss.default_background, pagesCss.content_position_center].join(' ')}>
            <FilesForm />
            
        </div>
    )
}

export default Files
