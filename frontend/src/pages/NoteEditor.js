import { useEffect, useState, useRef } from "react";
import { useParams } from "react-router-dom";
import { createNoteConnection } from "../apis/note/SignalR";
import {GetOneNote} from "../apis/note/GetOneNote";

export default function NoteEditor() {
    const { noteId } = useParams();
    const [content, setContent] = useState("");
    const [title, setTitle] = useState("");
    const [isConnected, setIsConnected] = useState(false);

    // Use refs for the connection and timeout so they persist across renders
    const connectionRef = useRef(null);
    const timeoutRef = useRef(null);

    // 1. Load the initial note data
    useEffect(() => {
        const loadNote = async () => {
            try {
                const res = await GetOneNote(noteId);
                setContent(res.content);
                setTitle(res.title);
            } catch (err) {
                console.error("Failed to load note:", err);
            }
        };
        if (noteId) {loadNote();}
        }, [noteId]);

    // 2. Manage SignalR Connection
    useEffect(() => {
        const conn = createNoteConnection();
        connectionRef.current = conn;

        async function start() {
            try {
                await conn.start();
                console.log("SignalR Connected");
                setIsConnected(true);

                // Join the specific room
                await conn.invoke("JoinNoteRoom", noteId);

                // Listen for updates from others
                conn.on("ReceiveNoteUpdate", (id, newContent) => {
                    if (id === noteId) {
                        setContent(newContent);
                    }
                });
            } catch (err) {
                console.error("SignalR Connection Error: ", err);
            }
        }

        start();

        return () => {
            if (conn) {
                conn.stop();
                setIsConnected(false);
            }
        };
    }, [noteId]);

    // 3. Handle Typing with Debounce
    const handleChange = (e) => {
        const newContent = e.target.value;
        setContent(newContent);

        // Clear existing timeout
        if (timeoutRef.current) clearTimeout(timeoutRef.current);

        // Only try to invoke if we are actually connected
        if (isConnected) {
            timeoutRef.current = setTimeout(() => {
                connectionRef.current
                    .invoke("EditNote", noteId, newContent)
                    .catch(err => console.error("EditNote failed: ", err));
            }, 500); // 500ms delay is usually smoother for DB saves
        }
    };

    return (
        <div style={{ padding: "40px" }}>
            <h2>{title} {isConnected ? "🟢" : "🔴"}</h2>
            <textarea
                value={content}
                onChange={handleChange}
                placeholder="Start typing..."
                style={{
                    width: "100%",
                    height: "80vh",
                    fontSize: "16px",
                    padding: "20px",
                    borderRadius: "8px"
                }}
            />
        </div>
    );
}