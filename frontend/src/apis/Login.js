import axios from "axios";

const API = process.env.REACT_APP_API_URL;

export async function loginUser(data) {
    const res = await axios.post(`${API}/auth/login`, data, {
        withCredentials: true,
        headers: {
            "Content-Type": "application/json",
        },
    });
    return res.data;
}