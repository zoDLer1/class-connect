import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Index from 'pages/index';
import Login from 'pages/login';
import Register from 'pages/register';
import Files from 'pages/files';
import { ClosingContext } from 'contexts/closingContext';
import  useClosing  from './hooks/useClosing'

function App() {
    const { add, remove, closeAll } = useClosing()

    return (
    <ClosingContext.Provider value={{add, remove}}>
        
        <BrowserRouter >
        <Routes>
          <Route  path='/' element={<Index />}></Route>
          <Route  path='/login' element={<Login />}></Route>
          <Route  path='/register' element={<Register />}></Route>
          <Route  path='/files' element={<Files  onContextMenu={() => closeAll()} onClick={() => closeAll()} />}></Route>
        </Routes>
      </BrowserRouter>
    </ClosingContext.Provider>
    
    )
}

export default App
