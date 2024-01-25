import React, { useState, useEffect, useRef } from 'react';
import "./Rating.css";
import { RatingItem } from './RatingItem';
export const Rating = () => {
    const [isPopupVisible, setPopupVisible] = useState(false);
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

    return (
        <>
            <button className="rating-btn" onClick={() => setPopupVisible(!isPopupVisible)}>Показать рейтинг</button>
            {isPopupVisible && (
                <div className="rating" ref={popupRef}>
                    <div className='rating-block'>
                        <h3>Рейтинг</h3>
                        <div className='rating-items-block'>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>
                            <RatingItem/>


                        </div>

                    </div>
                    
                </div>
            )}
        </>
)}