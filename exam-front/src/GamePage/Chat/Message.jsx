import React from 'react';

export const Message = ({message}) => {
    return(
        <>
            <div className='message'>
                {!message.isSystemMessage && message.username} : {message.message}
            </div>
            
        </>
        
    )
}
