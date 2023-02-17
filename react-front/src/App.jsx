import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Index from 'pages/index';
import Login from 'pages/login';
import Register from 'pages/register';
import Files from 'pages/files';



function App() {
    return (
    
    <BrowserRouter >
        <Routes>
          <Route  path='/' element={<Index />}></Route>
          <Route  path='/login' element={<Login />}></Route>
          <Route  path='/register' element={<Register />}></Route>
          <Route  path='/files' element={<Files />}></Route>
        </Routes>
      </BrowserRouter>
    )
}

export default App
