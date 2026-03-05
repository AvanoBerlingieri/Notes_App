import axios from "axios";
const API = process.env.REACT_APP_API_URL;

const apiClientWithCred = axios.create({
    baseURL: API,
    withCredentials: true,
    headers: {
        "Content-Type": "application/json",
    },
});

export default apiClientWithCred;