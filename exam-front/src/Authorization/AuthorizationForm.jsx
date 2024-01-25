import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { webApiFetcher } from "../axios/AxiosInstance";


export const AuthorizationForm = () => {
    const navigate = useNavigate();
    const [error, setError] = useState();
    const [credentials, setCredentials] = useState({
        username: "",
        password: ""
    });
    const updateCredentials = (name, value) => {
        credentials[name] = value;
        setCredentials({ ...credentials });
    };

    const handleSubmitForm = (event) => {
        event.preventDefault();
        setError("")

        webApiFetcher
          .post("auth/login", credentials)
          .then((res) => handleAuthorizationInfo(res.data))
          .catch((err) => handleError(err));
      };

      const handleAuthorizationInfo = (data) => {
        if (data) {
          localStorage.setItem("access-token", data.token);
          navigate("/");
        }
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
            <h2 className="reg-h2">Авторизация</h2>
            <form className="auth-form" onSubmit={handleSubmitForm}>
                <input
                    type="text"
                    placeholder="Имя пользователя"
                    onChange={(e) => updateCredentials("username", e.target.value.trim())}
                />
                <input
                    type="password"
                    placeholder="Пароль"
                    onChange={(e) => updateCredentials("password", e.target.value.trim())}
                />
                <input type="submit" value="Войти" />
                <div>{error}</div>
            </form>   
        </>
    )

}