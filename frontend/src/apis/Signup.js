import axios from "axios";

const API = process.env.REACT_APP_API_URL;

export async function signupUser(data) {
    const res = await axios.post(`${API}/auth/signup`, data, {
        headers: {"Content-Type": "application/json",},
    });
    return res.data;

}
