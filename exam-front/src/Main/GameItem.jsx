import dayjs from "dayjs"
import "./MainPage.css"
export const GameItem = ({game}) => {
    return (
        <>
            <div className="game-block">
                <div className="gameId-part">{game.gameId}</div>
                <div className="owner-part">{game.ownerName}</div>
                <div className="date-part">{dayjs(game.date).format("HH:mm:ss DD.MM.YY")}</div>
                <div className="rating-part">{game.maxRating}</div>
                <div className="enter-part">
                    <button className="enter-button">Войти</button>
                </div>
            </div>    
        </>
    )
}