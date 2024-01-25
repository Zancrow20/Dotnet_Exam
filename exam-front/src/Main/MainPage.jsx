import { CreateGame } from "../CreateGame/CreateGame"
import { Rating } from "../Rating/Rating"
import { GameBlock } from "./GameBlock"
import "./MainPage.css"
export const MainPage = () => {
    return (
        <>
        <div className="active-btns">
            <Rating/>
            <CreateGame/>
        </div>
        <div className="game-section">
            <GameBlock/>
            <GameBlock/>
        </div>
            
        </>
    )
}