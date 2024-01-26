import { useNavigate } from "react-router-dom";

export const useHandleError = () => {
    const navigate = useNavigate();

    return (err) => {
        if (err && err.response && err.response.status) {
            if (err.response.status == 401) navigate("/authorize");
        }
        console.log(err);
    }
    
}
