import { AuthorizationForm } from "./AuthorizationForm"
import "./AuthorizationPage.css";
export const AuthorizationPage = () => {
    return (
        <>
            <div className="auth-container">
                <AuthorizationForm/>
                <div>
                    Еще нет аккаунта? <a href="/register">Зарегестрируйся</a>
                </div>             
            </div>
        </>

    )
}