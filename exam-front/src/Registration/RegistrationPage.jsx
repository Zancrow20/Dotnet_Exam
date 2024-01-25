import { RegistrationForm } from "./RegistrationForm"
import "./RegistrationPage.css";

export const RegistrationPage = () => {
    return (
        <>
            <div className="auth-container">
                    <RegistrationForm/>  
                    <div>
                        Уже есть аккаунт? <a href="/authorize">Войти</a>
                    </div>           
            </div>
        </>

    )
}