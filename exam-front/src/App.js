import logo from './logo.svg';
import './App.css';
import { RegistrationPage } from './Registration/RegistrationPage';
import { AuthorizationPage } from './Authorization/AuthorizationPage';
import { MainPage } from './Main/MainPage';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { PageBuilder } from './PageBuilder/PageBuilder';
import { Rating } from './Rating/Rating';
import { GamePage } from './GamePage/GamePage';

function App() {
  return (
    <BrowserRouter>
        <Routes>
          <Route
            path="/game"
            element={<PageBuilder component={<GamePage/>}/>}
          />
          <Route
            path="/authorize"
            element={<PageBuilder component={<AuthorizationPage/>}/>}
          />
          <Route
            path="/register"
            element={<PageBuilder component={<RegistrationPage/>}/>}
          />
          <Route
            path="*"
            element={<PageBuilder component={<MainPage/>}/>}
          />
          
        </Routes>
      </BrowserRouter>
  );
}

export default App;
