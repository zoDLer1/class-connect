import { GlobalUIContext } from 'contexts/GlobalUIContext';
import { useAlert } from 'hooks/useAlert';
import Alert from 'components/UI/Alert';
import Popup from 'components/UI/Popup';
import Menu from 'components/UI/Menu';
import { usePopup } from 'hooks/usePopup';
import { useMenu } from 'hooks/useMenu';



const GlobalUI = ({ children }) => {
    const alert = useAlert()
    const popup = usePopup()
    const [menu, menuActions] = useMenu()

    return <GlobalUIContext.Provider value={{ alert, popup, menu:menuActions }}>
        <Alert {...alert} />
        <Menu {...menu} />
        <Popup {...popup} />
        {children}
    </GlobalUIContext.Provider>


}

export default GlobalUI;
