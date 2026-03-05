import { useEffect, useState } from "react";
import { GetAllNotes } from "../apis/note/GetAllNotes";
import { useNavigate } from "react-router-dom";
import "./css/Home.css";
import {CreateNote} from "../apis/note/CreateNote";

export default function Home() {

    const [notes, setNotes] = useState([]);
    const [search, setSearch] = useState("");
    const [filter, setFilter] = useState("all");
    const [sort, setSort] = useState("recent");
    const [showModal, setShowModal] = useState(false);
    const [newNote, setNewNote] = useState({
        title: "",
        collaborators: ""
    });

    const navigate = useNavigate();

    useEffect(() => {
        loadNotes();
    }, []);

    async function loadNotes() {
        const data = await GetAllNotes();
        setNotes(data);
    }

    const filteredNotes = notes
        .filter(note => {
            if (filter === "owned")
                return note.CurrentUserRole === "Owner";

            if (filter === "collab")
                return note.CurrentUserRole !== "Owner";

            return true;
        })
        .filter(note =>
            note.title.toLowerCase().includes(search.toLowerCase())
        )
        .sort((a, b) => {
            if (sort === "Alphabetical")
                return a.title.localeCompare(b.title);

            if (sort === "recent")
                return new Date(b.LastModified) - new Date(a.LastModified);

            return 0;
        });

    async function handleCreateNote() {
        if (!newNote.title) return;

        try {
            await CreateNote({
                title: newNote.title,
                content: " "
            });

            setShowModal(false);
            setNewNote({ title: "", collaborators: "" }); // Reset form
            loadNotes();
        } catch (err) {
            console.error("Failed to create note", err);
        }
    }

    return (
        <div className="home-container">

            <div className="toolbar">

                <button className="create-btn" onClick={() => setShowModal(true)}>New Note</button>

                <input
                    className="search-bar"
                    placeholder="Search notes..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                />

                <select
                    className="filter"
                    value={filter}
                    onChange={(e) => setFilter(e.target.value)}
                >
                    <option value="all">All Notes</option>
                    <option value="owned">Owned</option>
                    <option value="collab">Collaborations</option>
                </select>

                <select
                    className="sort"
                    value={sort}
                    onChange={(e) => setSort(e.target.value)}
                >
                    <option value="recent">Last Edited</option>
                    <option value="Alphabetical">Alphabetical</option>
                </select>

            </div>

            <div className="notes-grid">

                {filteredNotes.map(note => (
                    <div key={note.NoteId} className="note-card"
                         onClick={() => navigate(`/note/${note.NoteId}`)}>

                        <h3>{note.title}</h3>

                        <p>
                            Last edited: {new Date(note.LastModified).toLocaleDateString()}
                        </p>

                        <span className="role">
                            {note.CurrentUserRole}
                        </span>
                    </div>
                ))}

            </div>
            {showModal && (
                <div className="modal-overlay">
                    <div className="modal">

                        <h2>Create New Note</h2>

                        <input
                            placeholder="Note title"
                            value={newNote.title}
                            onChange={(e) =>
                                setNewNote({ ...newNote, title: e.target.value })
                            }
                        />

                        <textarea
                            placeholder="Add collaborators (comma separated emails)"
                            value={newNote.collaborators}
                            onChange={(e) =>
                                setNewNote({ ...newNote, collaborators: e.target.value })
                            }
                        />

                        <div className="modal-buttons">

                            <button
                                className="cancel-btn"
                                onClick={() => setShowModal(false)}
                            >
                                Cancel
                            </button>

                            <button
                                className="create-btn"
                                onClick={handleCreateNote}
                            >
                                Create
                            </button>

                        </div>

                    </div>
                </div>
            )}
        </div>
    );
}