import { useEffect, useState } from "react"
import { GameItem } from "./GameItem"
import { webApiFetcher } from "../axios/AxiosInstance";



export const GameBlock = () => {
    const [games, setGames] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [fetching, setFetching] = useState(true);
    const [lastPageNumb, setLastPageNumb] = useState(0);
    const pageSize = 8;

    useEffect(() => {
        if (fetching){  
            webApiFetcher.get(`game/lastPage?pageSize=${pageSize}`)
                .then(res => {setLastPageNumb(res.data)}) 
        }   
    }, [])


    useEffect(() => {
            if (fetching){  
                webApiFetcher.get(`game/all?pageNumber=${currentPage}&pageSize=${pageSize}`)
                    .then(res => {
                        setGames([...games, ...res.data.games]);
                        setCurrentPage(currentPage + 1);
                    })
                    .finally(() => setFetching(false));
            }
        
        }, [fetching])

        

    useEffect(() => {
            document.addEventListener('scroll', scrollHandler);
            return function(){
                document.removeEventListener('scroll', scrollHandler);
            };
        },[currentPage,lastPageNumb])


    const scrollHandler = (e) => {
        if (e.target.documentElement.scrollHeight - (e.target.documentElement.scrollTop + window.innerHeight) < 100 && currentPage <= lastPageNumb){
            setFetching(true);     
        }

    }




    return (
        <div className="game-section">
                <div className="games-header">
                    <div className="gameId-header">Id Игры</div>
                    <div className="owner-header">Имя пользователя</div>
                    <div className="date-header">Дата создания</div>
                    <div className="rating-header">Рейтинг</div>
                    <div className="enter-header"></div>
                </div>

                {games.map((game) => (
                    <GameItem key={game.gameId} game={game} />

                ))}
            </div>
    )
}