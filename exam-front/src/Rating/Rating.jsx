import React, { useState, useEffect, useRef } from 'react';
import "./Rating.css";
import { RatingItem } from './RatingItem';
import { webApiFetcher } from '../axios/AxiosInstance';
import leftArrow from "./media/left-arrow.png";
import rightArrow from "./media/right-arrow.png";
import { useHandleError } from '../ErrorHandler/ErrorHandler';
export const Rating = () => {
    const [isPopupVisible, setPopupVisible] = useState(false);
    const popupRef = useRef();
    const PageSize = 8;
    const [currentPage, setCurrentPage] = useState(1);
    const [lastPage, setLastPage] = useState(1);
    const [ratingList, setRatingList] = useState([]);
    const handleError = useHandleError();

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

    useEffect(() => {
        webApiFetcher
            .get(`rating/lastPage?pageSize=${PageSize}`)
            .then((res) => setLastPage((res.data)))
            .catch((err) => handleError(err));

        webApiFetcher
            .get(`rating/all?pageNumber=${currentPage}&pageSize=${PageSize}`)
            .then((res) => setRatingList((res.data.usersRating)))
            .catch((err) => handleError(err));
        
    }, [currentPage]);

    const leftClick = () =>{
        setCurrentPage(currentPage-1);
    }
    const rightClick = () =>{
        setCurrentPage(currentPage+1);
    }

    return (
        <>
            <button className="rating-btn" onClick={() => setPopupVisible(!isPopupVisible)}>Показать рейтинг</button>
            {isPopupVisible && (
                <div className="rating" ref={popupRef}>
                    <div className='rating-block'>
                        <h3>Рейтинг</h3>
                        <div className='rating-items-block'>
                            <div>
                                {ratingList.map((ratingItem) => (
                                    <RatingItem ratingItem={ratingItem} />
                                ))}
                            </div>
                        </div>
                        <div className='rating-pagination-block'>
                                <img
                                    className={'left-arrow ' + ( (currentPage === 1)?'non-visible':'')}
                                    src={leftArrow}
                                    alt="Влево"
                                    onClick={() => {
                                        leftClick();
                                    }}/>
                                <div className='rating-page-numb'>{currentPage}</div>
                                <img
                                    className={'right-arrow ' + ((currentPage === lastPage)?'non-visible':'')}
                                    src={rightArrow}
                                    alt="Вправо"
                                    onClick={() => {
                                        rightClick();
                                    }}/>
                            </div>
                    </div>
                </div>
            )}
        </>
)}