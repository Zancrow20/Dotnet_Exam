import { ChatInput } from "./ChatInput";
import { ChatWindow } from "./ChatWindow";
import "./Chat.css"

export const Chat = (props) => {
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