import apiClient from ".././ApiClientWithCred";

export async function logoutUser() {
    const res = await apiClient.post("/auth/logout");
    return res.data;
}