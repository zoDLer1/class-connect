import { Link } from "react-router-dom";



function BaseLink({to, text, ...props}) {
    return (
        <>
            {to 
                ? <Link to={to} {...props}>{text}</Link>
                : <p {...props}>{text}</p>
            }
        </>   
    )
}

export default BaseLink
