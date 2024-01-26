import React from 'react';

export const Message = ({message}) => {
    return(
        <>
            <div className='message'>
                {message.username} : {message.message}
            </div>
            
        </>
        
    )
}
