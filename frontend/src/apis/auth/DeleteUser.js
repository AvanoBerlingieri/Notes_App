import apiClient from ".././ApiClientWithCred";

export async function DeleteUser() {
    const res = await apiClient.delete("/auth/user");
    return res.status;
}