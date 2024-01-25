import axios from "axios";


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