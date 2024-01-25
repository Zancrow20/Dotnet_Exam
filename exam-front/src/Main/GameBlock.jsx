import { GameItem } from "./GameItem"

export const GameBlock = ({games}) => {
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
                    <GameItem game={game} />
                ))}
            </div>
    )
}