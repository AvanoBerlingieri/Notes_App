import axios from "axios";
const API = process.env.REACT_APP_API_URL;

export async function logoutUser() {
    const res = await axios.post(`${API}/auth/logout`, {},
        {
            withCredentials: true
        });
    return res.data;
}