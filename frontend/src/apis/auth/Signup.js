import apiClient from ".././ApiClientWithoutCred";

export async function signupUser(data) {
    const res = await apiClient.post("/auth/signup", data);
    return res.data;
}