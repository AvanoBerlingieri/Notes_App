import apiClient from ".././ApiClientWithCred";

export async function EditNote(noteId, data) {
    const res = await apiClient.put(`/notes/${noteId}`, data);
    return res.data;
}