import React, { useEffect, useRef, useState } from "react";
import { webApiFetcher } from "../axios/AxiosInstance";
import { useSearchParams, useNavigate } from "react-router-dom";
import "./GamePage.css";
import rock from "./media/Rock.png";
import paper from "./media/Paper.png";
import scissors from "./media/Scissors.png";
import { Chat } from "./Chat/Chat";
import { useHandleError } from "../ErrorHandler/ErrorHandler";
import Ports from "../consts/Ports";
import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
import { getSimbol } from "../GetSimbol";


export const GamePage = () => {
  const navigate = useNavigate();
  const [isLoad, setIsLoad] = useState(false);
  const [connection, setConnection] = useState(null);
  const [searchParams] = useSearchParams();
  const handleError = useHandleError();
  const [simbol, setSimbol] = useState(10);
  const [chat, setChat] = useState([]);
  const latestChat = useRef(null);
  const [isPlayer, setIsPlayer] = useState(false);
  const [gameState, setGameState] = useState(0);
  const [finishMessage, setFinishMessage] = useState("")
  const [resultString, setResultString] = useState("")
  const simb = useRef(10);

  latestChat.current = chat;

  function updateHistory() {
    webApiFetcher.get(`/chat/getHistory?gameId=${searchParams.get("gameId")}`)
      .then(result => setChat(result.data.chatMessageDtos))
      .catch(ex => console.log(ex));
  }

  useEffect(() => {
    if (searchParams.get("gameId")) {
      webApiFetcher
        .get(`game?gameId=${searchParams.get("gameId")}`)
        .then((res) => {
          setGameState(res.data.status)
          setIsLoad(true);
        })
        .catch((err) => {
          if (err && err.response && err.response.status) {
            if (err.response.status == 400) navigate("/");
          }
          else handleError(err);
        })
    }

    const authToken = localStorage.getItem("access-token") ?? "";
    const newConnection = new HubConnectionBuilder()
      .withUrl(`http://localhost:${Ports.WebApi}/gamehub`, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: () => `${authToken}`
      })
      .withAutomaticReconnect()
      .build();
    setConnection(newConnection)
    updateHistory();

  }, [searchParams]);


  useEffect(() => {
    if (connection) {
      connection.start()
        .then((result) => {
          console.log('Connected!');
          connection.send("WatchGame", searchParams.get("gameId"));

          connection.on('StartGame', () => {
            setGameState(1);
          });

          connection.on('SuccessJoin', () => {
            console.log("ты игрок");
            setIsPlayer(true);
          });

          connection.on('ReceiveMessage', message => {
            const updatedChat = [...latestChat.current];
            updatedChat.push(message);
            setChat(updatedChat);
          });

          connection.on("AskFigure", () => {
            console.log("отправка хода")
            makeMove();
            setGameState(2)
          });

        
          connection.on("JoinRefused", (message) => alert(message))

          connection.on("FinishGame", finishDto => {
            console.log(finishDto);
            setFinishMessage(finishDto.message);
            setResultString(`${finishDto.winnerName}: ${getSimbol(finishDto.winnerFigure)} vs ${finishDto.loserName}: ${getSimbol(finishDto.loserFigure)}`)
          })
        })
        .catch(e => console.log('Connection failed: ', e));
    }
  }, [connection]);




  const enterToGame = async () => {
    if (connection._connectionStarted) {
      try {
        await connection.send('JoinGame', searchParams.get("gameId"));
      }
      catch (e) {
        console.log(e);
      }
    }
    else {
      alert('No connection to server yet.');
    }
  }

  const sendMessage = async (message) => {
    const chatMessage = {
      gameId: searchParams.get("gameId"),
      message: message
    };

    if (connection._connectionStarted) {
      try {
        await connection.send('SendMessage', searchParams.get("gameId"), message);
      }
      catch (e) {
        console.log(e);
      }
    }
    else {
      alert('No connection to server yet.');
    }
  }

  const makeMove = async () => {

    if (connection._connectionStarted) {
      try {
        console.log(simb.current)
        if (simb.current === 10) {
          await connection.send('MakeMove', searchParams.get("gameId"), Math.floor(Math.random() * 3));
        }
        else {
          await connection.send('MakeMove', searchParams.get("gameId"), simbol.current);
        }

      }
      catch (e) {
        console.log(e);
      }
    }
    else {
      alert('No connection to server yet.');
    }
  }

  return (
    isLoad && (
      <>
        <div>
          {(!isPlayer && gameState === 0) && (<button onClick={enterToGame} className="enter-game-btn">Присоединиться</button>)}
          {(gameState === 0) && <span>Игра создана. Ожидание игроков.</span>}
          {(gameState === 2) && <p>Игра завершена. {finishMessage}</p>}
          {(gameState === 2) && <p>{resultString}</p>}
          {(gameState === 1) && <span>Идет игра. Игрокам нужно выбрать символ. На выбор дается 10 секунд</span>}
          <br />
          {(gameState === 1 && isPlayer) && <span>Ваш символ: {getSimbol(simbol)}</span>}

          <div className="btn-panel">
            <button onClick={() => {setSimbol(0); simb.current = 0} } className="rock-btn">
              <img className="rock-btn-img" src={rock} alt="Камень" />
            </button>
            <button onClick={() => {setSimbol(1); simb.current = 1}} className="scissors-btn">
              <img className="scissors-btn-img" src={scissors} alt="Ножницы" />
            </button>
            <button onClick={() => {setSimbol(2); simb.current = 2}} className="paper-btn">
              <img className="paper-btn-img" src={paper} alt="Бумага" />
            </button>
          </div>
        </div>

        <Chat chat={chat} sendMessage={sendMessage} />
      </>
    )
  );
};
