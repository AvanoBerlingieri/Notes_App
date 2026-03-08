import apiClient from ".././ApiClientWithCred";

export async function UpdateEmail(info) {
    const res = await apiClient.put("/auth/user/email", info);
    return res.status;
}