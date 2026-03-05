import axios from "axios";
const API = process.env.REACT_APP_API_URL;

const apiClientWithoutCred = axios.create({
    baseURL: API,
    withCredentials: false,
    headers: {
        "Content-Type": "application/json",
    },
});

export default apiClientWithoutCred;