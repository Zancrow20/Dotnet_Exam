import "./PageBuilder.css";


export const PageBuilder = ({ component }) => {
  
  return (
    <>
        <header>
          <div className="content-header">Камень, ножницы, бумага</div>
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
