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


export const GamePage = () => {
  const navigate = useNavigate();
  const [isLoad, setIsLoad] = useState(false);
  const [connection, setConnection] = useState(null);
  const [searchParams] = useSearchParams();
  const handleError = useHandleError();
  const [simbol, setSimbol] = useState(10);
  const [ chat, setChat ] = useState([]);
  const latestChat = useRef(null);
  const [isPlayer, setIsPlayer] = useState(false);
  const [isGameProcess, setIsGameProcess] = useState(false);

  useEffect(() => {
    if (searchParams.get("gameId")) {
      webApiFetcher
        .get(`game?gameId=${searchParams.get("gameId")}`)
        .then((res) => {
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
    //updateHistory();

  }, [searchParams]);

  useEffect(() => {
    if (isGameProcess){
      runGameTimer();
    }
  }, [isGameProcess])

  useEffect(() => {
    if (connection) {
      console.log(connection)
      connection.start()
        .then((result) => {
          console.log('Connected!');
          connection.send("WatchGame",searchParams.get("gameId") );

          connection.on('StartGame', () => {
            setIsGameProcess(true);
          });

          connection.on('SuccessJoin', () => {
            console.log("ты игрок");
            setIsPlayer(true);
          });


          connection.on('ReceiveMessage', message => {
            console.log("recieve")
            const updatedChat = [...latestChat.current];
            updatedChat.push(message);
            setChat(updatedChat);
          });

          connection.on("JoinRefused", (message) => console.log(message))
        })
        .catch(e => console.log('Connection failed: ', e));
    }
  }, [connection]);

  const enterToGame = async () => {
    console.log("s")
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

  function runGameTimer() {
    console.log("run timer")
    const timer = setTimeout(() => {
      makeMove();
      setIsGameProcess(false)
    }, 12000);
  
  }


  const sendMessage = async (message) => {
    const chatMessage = {
        gameId: searchParams.get("gameId"),
        message: message
    };

    if (connection._connectionStarted) {
        try {
          console.log(chatMessage)
            await connection.send('SendMessage', searchParams.get("gameId"),message);
        }
        catch(e) {
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
          if (simbol === 10) setSimbol(Math.floor(Math.random() * 3));
          await connection.send('MakeMove', searchParams.get("gameId"),simbol);
        }
        catch(e) {
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
          {!isPlayer && (<button onClick={enterToGame} className="enter-game-btn">Присоединиться</button>)}
          {isGameProcess && <span>Выберете один из значков снизу. На выбор дается 10 секунд</span>}
          <div className="btn-panel">
            <button onClick={() => setSimbol(0)} className="rock-btn">
              <img className="rock-btn-img" src={rock} alt="Камень" />
            </button>
            <button onClick={() => setSimbol(1)} className="scissors-btn">
              <img className="scissors-btn-img" src={scissors} alt="Ножницы" />
            </button>
            <button onClick={() => setSimbol(2)} className="paper-btn">
              <img className="paper-btn-img" src={paper} alt="Бумага" />
            </button>
          </div>
        </div>

        <Chat chat={chat} sendMessage={sendMessage} />
      </>

    )
  );
};
