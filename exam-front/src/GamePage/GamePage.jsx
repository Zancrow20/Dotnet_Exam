import React, { useEffect, useState } from "react";
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
  const [simbol, setSimbol] = useState("");

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
      .withUrl(`https://localhost:${Ports.WebApi}/chat`, {
        transport: HttpTransportType.WebSockets,
        skipNegotiation: true,
        accessTokenFactory: () => `${authToken}`
      })
      .withAutomaticReconnect()
      .build();
    setConnection(newConnection)
    //updateHistory();

  }, [searchParams]);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then((result) => {
          console.log('Connected!');
          connection.send("WatchGame");

          connection.on('RecieveStartGame', () => {
              runGameTimer();
          });
          connection.on('RecieveChatMessage', message => {
            //const updatedChat = [...latestChat.current];
            //updatedChat.push(message);
            //setChat(updatedChat);
          });
        })
        .catch(e => console.log('Connection failed: ', e));
    }
  }, [connection]);

  const enterToGame = async (gameId) => {
    if (connection._connectionStarted) {
      try {
        await connection.send('JoinGame', gameId);
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
    const timer = setTimeout(() => {
      выполнитьДругойМетод();
    }, 7000);
  
  }
  
  function выполнитьДругойМетод() {
    console.log("Метод выполнен после 7 секунд");
  }
  
  






  return (
    isLoad && (
      <>
        <div>
          <button onClick={enterToGame} className="enter-game-btn">Присоединиться</button>
          <div className="btn-panel">
            <button onClick={() => setSimbol("Камень")} className="rock-btn">
              <img className="rock-btn-img" src={rock} alt="Камень" />
            </button>
            <button onClick={() => setSimbol("Ножницы")} className="scissors-btn">
              <img className="scissors-btn-img" src={scissors} alt="Ножницы" />
            </button>
            <button onClick={() => setSimbol("Бумага")} className="paper-btn">
              <img className="paper-btn-img" src={paper} alt="Бумага" />
            </button>
          </div>
        </div>

        <Chat />
      </>

    )
  );
};
