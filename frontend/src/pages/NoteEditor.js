import {useEffect, useState, useRef} from "react";
import {useParams} from "react-router-dom";
import {useEditor, EditorContent} from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import Underline from "@tiptap/extension-underline";
import Color from "@tiptap/extension-color";
import Highlight from "@tiptap/extension-highlight";
import FontFamily from "@tiptap/extension-font-family";
import TextAlign from "@tiptap/extension-text-align";
import TaskList from "@tiptap/extension-task-list";
import TaskItem from "@tiptap/extension-task-item";
import {CustomTextStyle} from "../components/editor/CustomTextStyle";

import Topbar from "../components/layout/Topbar";
import EditorToolbar from "../components/editor/EditorToolbar";
import {createNoteConnection} from "../apis/note/SignalR";
import {GetOneNote} from "../apis/note/GetOneNote";

import "../pages/css/NoteEditor.css";

export default function NoteEditor() {
    const {noteId} = useParams();

    // const [users, setUsers] = useState([]);
    const [title, setTitle] = useState("");
    const [isConnected, setIsConnected] = useState(false);
    const [saveStatus, setSaveStatus] = useState("Saved");

    const connectionRef = useRef(null);
    const timeoutRef = useRef(null);

    const editor = useEditor({
        extensions: [
            StarterKit,
            Underline,
            CustomTextStyle,
            Color,
            Highlight,
            FontFamily,
            TextAlign.configure({types: ["paragraph", "heading"]}),
            TaskList,
            TaskItem.configure({nested: true}),
        ],
        content: "",
        onUpdate: ({editor}) => {
            const html = editor.getHTML();
            setSaveStatus("Typing...");
            if (timeoutRef.current) clearTimeout(timeoutRef.current);

            if (isConnected) {
                timeoutRef.current = setTimeout(() => {
                    setSaveStatus("Saving...");
                    connectionRef.current
                        .invoke("EditNote", noteId, html)
                        .then(() => setSaveStatus("Saved"))
                        .catch(err => console.error("EditNote failed:", err));
                }, 750);
            }
        },
    });

    useEffect(() => {
        const loadNote = async () => {
            try {
                const res = await GetOneNote(noteId);
                setTitle(res.title);
                if (editor && res.content) editor.commands.setContent(res.content);
            } catch (err) {
                console.error("Failed to load note:", err);
            }
        };
        if (noteId && editor) loadNote();
    }, [noteId, editor]);

    useEffect(() => {
        const conn = createNoteConnection();
        connectionRef.current = conn;

        async function start() {
            try {
                await conn.start();
                setIsConnected(true);
                await conn.invoke("JoinNoteRoom", noteId);

                conn.on("ReceiveNoteUpdate", (id, newContent) => {
                    if (id === noteId && editor) {
                        const current = editor.getHTML();
                        if (current !== newContent) editor.commands.setContent(newContent);
                    }
                });

            } catch (err) {
                console.error(err);
            }
        }

        start();
        return () => {
            if (conn) conn.stop();
        };
    }, [noteId, editor]);

    return (
        <div className="note-page">

            <Topbar
                title={title}
                username={"Avano"}
                saveStatus={saveStatus}
                showBack={true}
                showSearch={false}
            />

            <EditorToolbar editor={editor}/>
            <div className="editor-container"><EditorContent editor={editor}/></div>
        </div>
    );
}