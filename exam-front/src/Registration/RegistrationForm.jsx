import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { getFetcher } from "../axios/AxiosInstance";
import Ports from "../consts/Ports";

const fetcher = getFetcher(Ports.ExamServer);
export const RegistrationForm = () => {
    const navigate = useNavigate();
    const [credentials, setCredentials] = useState({
        username: "",
        password: "",
        confirmPassword: ""
    });
    const updateCredentials = (name, value) => {
        credentials[name] = value.trim();
        setCredentials({ ...credentials });
      };
    const [error, setError] = useState();

    const handleSubmitForm = (event) => {
        event.preventDefault();
        setError("");
    
        fetcher
          .post("auth/register", credentials)
          .then((res) => navigate("/authorize"))
          .catch((err) => handleError(err));
      };
      const handleError = (err) => {
        if (err && err.response && err.response.data) {
            setError(err.response.data);
            return;
        }
        setError(err.message);
      };
    return (
        <>
            <div>
                <h2 className="reg-h2">Регистрация</h2>
                <form className="register-form" onSubmit={handleSubmitForm}>
                    <input
                        type="text"
                        placeholder="Имя пользователя"
                        onChange={(e) => updateCredentials("username", e.target.value)}
                    />
                    <input
                        type="password"
                        placeholder="Пароль"
                        onChange={(e) => updateCredentials("password", e.target.value)}
                    />
                    <input
                        type="password"
                        placeholder="Повторите пароль"
                        onChange={(e) => updateCredentials("confirmPassword", e.target.value)}
                    />
                    <input type="submit" value="Зарегестрироваться" />
                    <div>{error}</div>
                </form> 

            </div>
            
        </>
    )


}