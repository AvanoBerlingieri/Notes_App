import apiClient from ".././ApiClientWithCred";

export async function GetOneNote(noteId) {
    const res = await apiClient.get(`/notes/${noteId}`);
    return res.data;
}