import apiClient from ".././ApiClientWithCred";

export async function UpdateName(info) {
    const res = await apiClient.put("/auth/user/name", info);
    return res.status;
}