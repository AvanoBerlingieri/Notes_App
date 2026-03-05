import apiClient from "./ApiClientWithCred";

export async function loginUser(data) {
    const res = await apiClient.post("/auth/login", data);
    return res.data;
}