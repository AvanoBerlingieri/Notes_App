import apiClient from ".././ApiClientWithCred";

export async function DeleteNote(noteId) {
    const res = await apiClient.delete(`/notes/${noteId}`);
    return res.data;
}