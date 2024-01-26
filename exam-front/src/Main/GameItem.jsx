import dayjs from "dayjs"
import "./MainPage.css"
import { createSearchParams, useNavigate } from "react-router-dom"
import { getGameStatus } from "../GetGameStatus";


const useNavigateSearch = () => {
    const navigate = useNavigate();
    return (pathname, params) =>
      navigate({ pathname, search: `?${createSearchParams(params)}` });
  };

const prefix = `https://localhost:3000`
export const GameItem = ({game}) => {

    const navigateSearch = useNavigateSearch();
    const OnEnterClick = (gameId) =>{
        navigateSearch("/game", { gameId: gameId });
    }
    
    return (
        <>
            <div className="game-block">
                <div className="gameId-part">{game.gameId}</div>
                <div className="owner-part">{game.ownerName}</div>
                <div className="date-part">{dayjs(game.date).format("HH:mm:ss DD.MM.YY")}</div>
                <div className="rating-part">{game.maxRating}</div>
                <div className="status-part">{getGameStatus(game.status)}</div>
                <div className="enter-part">
                    <button className="enter-button" onClick={() => OnEnterClick(game.gameId)}>Войти</button>
                </div>
            </div>    
        </>
    )
}