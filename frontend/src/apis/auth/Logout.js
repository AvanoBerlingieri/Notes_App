import apiClient from ".././ApiClientWithCred";
import {useAuth} from "../../context/AuthContext";

const { setUser, setAuthenticated } = useAuth();

export async function logoutUser() {
    const res = await apiClient.post("/auth/logout");

    setUser(null);
    setAuthenticated(false);
    return res.data;
}