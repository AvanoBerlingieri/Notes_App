import apiClient from ".././ApiClientWithCred";

export async function CreateNote(data) {
    const res = await apiClient.post("/notes", data);
    return res.data;
}