import React, { useState, useEffect, useRef } from 'react';
import "./CreateGame.css";
export const CreateGame = () => {
    const [isPopupVisible, setPopupVisible] = useState(false);
    const [minRating, setMinRating] = useState(100);
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
    
        // fetcher
        //   .post("auth/register", credentials)
        //   .then((res) => navigate("/authorize"))
        //   .catch((err) => handleError(err));
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
                                value={minRating}
                                onChange={(e) => setMinRating(e.target.value)}
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