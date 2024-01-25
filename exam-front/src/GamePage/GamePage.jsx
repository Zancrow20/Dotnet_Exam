import React, { useEffect, useState } from "react";
import { getFetcher } from "../axios/AxiosInstance";
import Ports from "../consts/Ports";
import { useSearchParams, useNavigate } from "react-router-dom";
import "./GamePage.css";
import rock from "./media/Rock.png";
import paper from "./media/Paper.png";
import scissors from "./media/Scissors.png";

const fetcher = getFetcher(Ports.ExamServer);

export const GamePage = () => {
  const navigate = useNavigate();
  const [isLoad, setIsLoad] = useState(false);
  const [searchParams] = useSearchParams();

  useEffect(() => {
    console.log(searchParams)
    if (searchParams.get("gameId")) {
        setIsLoad(true);
    //   fetcher
    //     .get(`game/get/${searchParams.get("playlistId")}`)
    //     .then((res) => { 
    //       console.log(res.data);
          
    //     })
    //     .catch((err) => {
    //       if (err.response.status === 401) navigate("/authorize");
    //       else{
    //         console.log(err);
    //       }
    //     });
    }
  }, [searchParams]);




  return (
    isLoad && (
        <div>
            <img
                src={rock}
                alt="Камень"
                onClick={() => {
                  //backClick();
                }}
              />
              <img
                src={scissors}
                alt="Ножницы"
                onClick={() => {
                  //backClick();
                }}
              />
              <img
                src={paper}
                alt="Бумага"
                onClick={() => {
                  //backClick();
                }}
              />
            {/* <button className="rock-btn"></button>
            <button className="paper-btn"></button>
            <button className="scissors-btn"></button> */}
        </div>
        
    )
  );
};
