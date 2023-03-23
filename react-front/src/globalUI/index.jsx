import { GlobalUIContext } from 'contexts/GlobalUIContext';
import { useAlert } from 'hooks/useAlert';
import Alert from 'components/UI/Alert';
import Popup from 'components/UI/Popup';
import { usePopup } from 'hooks/usePopup';



const GlobalUI = ({ children }) => {
    const alert = useAlert()
    const popup = usePopup()
    return (
        <>
            
            <GlobalUIContext.Provider value={{ alert, popup }}>
                <Alert {...alert} />
                <Popup {...popup}/>
                {children}
            </GlobalUIContext.Provider>
        </>
    );
}

export default GlobalUI;
