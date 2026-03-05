import * as signalR from "@microsoft/signalr";
const API = process.env.REACT_APP_API_URL;

export const createNoteConnection = () => {

    return new signalR.HubConnectionBuilder().withUrl(`${API}/noteHub`, {
            withCredentials: true
        })
        .withAutomaticReconnect()
        .build();
};