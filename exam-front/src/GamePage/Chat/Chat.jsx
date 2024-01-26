import { ChatInput } from "./ChatInput";
import { ChatWindow } from "./ChatWindow";
import "./Chat.css"

export const Chat = () => {
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

    const sendMessage = () => {

    }

    return (
        <div className="chat-container">
            <h3>Чат</h3>
            <div className='chat-block'>
                <ChatWindow chat={chat}/>
                <ChatInput sendMessage={sendMessage} />
            </div>
        </div>
    );
}