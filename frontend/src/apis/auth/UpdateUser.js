import apiClient from ".././ApiClientWithCred";

export async function UpdateUser(info) {
    const res = await apiClient.put("/auth/user", info);
    return res.status;
}