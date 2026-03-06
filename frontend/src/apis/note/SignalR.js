import * as signalR from "@microsoft/signalr";

export const createNoteConnection = () => {

    return new signalR.HubConnectionBuilder().withUrl("http://localhost:8080/NoteHub", {
            withCredentials: true
        })
        .withAutomaticReconnect()
        .build();
};