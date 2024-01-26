import { ChatInput } from "./ChatInput";
import { ChatWindow } from "./ChatWindow";
import "./Chat.css"

export const Chat = (props) => {
    const chat = [
        {
            username: "Ola",
            message: "message1"
        },
        {
            username: "LOla",
            message: "message2"
        },
        {
            username: "LOlo",
            message: "message3"
        },
    ]

    

    return (
        <div className="chat-container">
            <h3>Чат</h3>
            <div className='chat-block'>
                <ChatWindow chat={props.chat}/>
                <ChatInput sendMessage={props.sendMessage} />
            </div>
        </div>
    );
}