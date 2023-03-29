import Index from 'pages/index';
import Login from 'pages/login';
import Register from 'pages/register';
import Files from 'pages/files';
import { Routes, Route, BrowserRouter } from 'react-router-dom';
import GlobalUI from 'globalUI';
import GroupEnter from 'pages/groupEnter';
import RootNotFound from 'pages/rootNotFound';

const Router = () => {
    return (


        <BrowserRouter>
            <GlobalUI>
                <Routes>
                    <Route path='/group/enter/:id' element={<GroupEnter />}/>
                    <Route path='/' element={<Index />}></Route>
                    <Route path='/login' element={<Login />}></Route>
                    <Route path='/register' element={<Register />}></Route>
                    <Route path='/files/:id' element={<Files />}></Route>
                    <Route path='/rootNotFound' element={<RootNotFound />}></Route>
                </Routes>
            </GlobalUI>
        </BrowserRouter>


    );
}

export default Router;
