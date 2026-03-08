import apiClient from ".././ApiClientWithCred";

export async function ChangePassword(passwordDto) {
    const res = await apiClient.patch("/auth/user/pass", passwordDto);
    return res.status;
}