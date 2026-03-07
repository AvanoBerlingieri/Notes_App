import apiClient from ".././ApiClientWithCred";

export async function GetUser() {
    const res = await apiClient.get("/auth/user");
    return res.data;
}