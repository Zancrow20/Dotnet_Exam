import axios from "axios";
import Ports from "../consts/Ports";


export const getFetcher = (port) => {
  const instance = axios.create({
    baseURL: `http://localhost:${port}/`,
  });

  instance.interceptors.request.use((config) => {
    const authToken = localStorage.getItem("access-token") ?? "";
    config.headers.Authorization = `Bearer ${authToken}`;
    return config;
  });
  return instance;
};

export const webApiFetcher = getFetcher(Ports.WebApi);