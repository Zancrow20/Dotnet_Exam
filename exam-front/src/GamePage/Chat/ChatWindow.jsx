import React from 'react';
import { Message } from './Message';


export const ChatWindow = ({chat}) => {
    return(
        <div className='messages-block'>
            {chat.map((m,id) => <Message key={id * Math.random()} message={m}/>)}
        </div>
    )
};