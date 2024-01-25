export const RatingItem = ({ratingItem}) => {
    return (
        <div className="rating-item">
            <div>{ratingItem.username}</div>
            <div>{ratingItem.rating}</div>
        </div>
    )
}