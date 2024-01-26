import { useEffect, useState } from "react"
import { CreateGame } from "../CreateGame/CreateGame"
import { Rating } from "../Rating/Rating"
import "./MainPage.css"
import { webApiFetcher } from "../axios/AxiosInstance"
import { useNavigate } from "react-router-dom"
import { GameBlock } from "./GameBlock"
import { useHandleError } from "../ErrorHandler/ErrorHandler"


export const MainPage = () => {
    const [username, setUsername] = useState("");
    const [userRating, setUserRating] = useState(0);
    const handleError = useHandleError();

    useEffect(() => {
        webApiFetcher
            .get("user")
            .then((res) => {
                setUsername(res.data.username);
                setUserRating(res.data.rating);
            })
            .catch((err) => handleError(err));
    }, []);

    return (
        <>
            <div className="active-btns">
                <Rating/>
                <CreateGame/>
            </div>
            <div className="greeting">Здравствуйте, {username}! Ваш рейтинг: {userRating}</div>
            <GameBlock/>      
        </>
    )
}