import React, { useState, useEffect, useRef } from 'react';
import "./CreateGame.css";
import { webApiFetcher } from '../axios/AxiosInstance';
import { useNavigate } from 'react-router-dom';
export const CreateGame = () => {
    const [isPopupVisible, setPopupVisible] = useState(false);
    const [maxRating, setMaxRating] = useState(1000);
    const navigate = useNavigate();
    const popupRef = useRef();
    useEffect(() => {
        const handleOutsideClick = (event) => {
            if (popupRef.current && !popupRef.current.contains(event.target)) {
                setPopupVisible(false);
            }
        }
        document.addEventListener('mousedown', handleOutsideClick);
        return () => {
            document.removeEventListener('mousedown', handleOutsideClick);
        }
    }, []);

    const [error, setError] = useState("");

    const handleSubmitForm = (event) => {
        event.preventDefault();
        setError("");
        webApiFetcher
          .post('game/create?maxRating='+maxRating)
          .then((res) => {
            if (res.status == 200) window.location.reload()
          } )
          .catch((err) => handleError(err));
      };
      const handleError = (err) => {
        if (err && err.response && err.response.data) {
            setError(err.response.data);
            return;
        }
        setError(err.message);
      };

    return (
        <>
            <button className="create-game-btn" onClick={() => setPopupVisible(!isPopupVisible)}>Создать игру</button>
            {isPopupVisible && (
                <div className="create-game" ref={popupRef}>
                    <div className='create-game-block'>
                        <h3>Создание игры</h3>
                        <form className="create-game-form" onSubmit={handleSubmitForm} >
                            <span>Минимальный рейтинг</span>
                            <input
                                type="number"
                                value={maxRating}
                                onChange={(e) => setMaxRating(e.target.value)}
                            />
                            <div className='error-message'>{error}</div>
                            <input type="submit" value="Создать" />
                        </form> 
                        

                    </div>
                    
                </div>
            )}
        </>
    )
}