import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Index from 'pages/index';
import Login from 'pages/login';
import Register from 'pages/register';
import Files from 'pages/files';
import { CloseContext } from 'contexts/сloseContext';
import { useClose } from 'hooks/useClose';
import { AlertContext } from 'contexts/alertContext';
import { useAlert } from 'hooks/useAlert';
import Alert from 'components/UI/Alert';

function App() {

  const { add, remove, closeAll } = useClose()
  
  const hook = useAlert()


    return (
    <CloseContext.Provider value={{add, remove, closeAll}}>
      <AlertContext.Provider value={hook.show}>
        <main onClick={()=>closeAll()} onContextMenu={(evt)=>{evt.preventDefault(); closeAll()}}>
          <Alert hook={hook}/>
          <BrowserRouter >
          <Routes>
            <Route  path='/' element={<Index />}></Route>
            <Route  path='/login' element={<Login />}></Route>
            <Route  path='/register' element={<Register />}></Route>
            <Route  path='/files' element={<Files  />}></Route>
          </Routes>
        </BrowserRouter>
        </main>
      </AlertContext.Provider>
    </CloseContext.Provider>
    
    )
}

export default App
