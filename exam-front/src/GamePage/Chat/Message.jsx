import React from 'react';

export const Message = ({message}) => {
    return(
        <>
            <div className='message'>
                {message.from} : {message.message}
            </div>
            
        </>
        
    )
}
