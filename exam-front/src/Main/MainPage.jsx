import { useEffect, useState } from "react"
import { CreateGame } from "../CreateGame/CreateGame"
import { Rating } from "../Rating/Rating"
import { GameBlock } from "./GameBlock"
import "./MainPage.css"
import { webApiFetcher } from "../axios/AxiosInstance"
import { useNavigate } from "react-router-dom"


export const MainPage = () => {
    const [username, setUsername] = useState("");
    const [userRating, setUserRating] = useState(0);
    const navigate = useNavigate();
    useEffect(() => {
        webApiFetcher
            .get("user")
            .then((res) => {
                setUsername(res.data.username);
                setUserRating(res.data.rating);
            })
            .catch((err) => handleError(err));
    }, []);

    const handleError = (err) => {
        if (err && err.response && err.response.status) {
            if (err.response.status == 401) navigate("/authorize");
        }
        console.log(err);
      };

    return (
        <>
            <div className="active-btns">
                <Rating/>
                <CreateGame/>
            </div>
            <div className="greeting">Здравствуйте, {username}! Ваш рейтинг: {userRating}</div>
            <div className="game-section">
                <GameBlock/>
                <GameBlock/>
            </div>      
        </>
    )
}