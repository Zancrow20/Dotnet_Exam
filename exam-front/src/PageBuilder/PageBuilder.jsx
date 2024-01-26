import "./PageBuilder.css";
import logo from "./media/logo.png"


export const PageBuilder = ({ component }) => {
  
  return (
    <>
        <header>
          <div className="header-container">
            <img src={logo} alt="logo" className="header-logo"></img>
            <div className="content-header">Камень, ножницы, бумага</div> 
          </div>
          
        </header>
        <div className="content">
          <main className="main-page"> 
            {component}
          </main>
        </div>
        <footer>
          <div className="content-footer"></div>
        </footer>
    </>
  );
};
