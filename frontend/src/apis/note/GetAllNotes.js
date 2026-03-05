import apiClient from ".././ApiClientWithCred";

export async function GetAllNotes() {
    const res = await apiClient.get("/notes");
    return res.data;
}