import {useEffect, useState} from "react";
import {GetAllNotes} from "../apis/note/GetAllNotes";
import {EditNote} from "../apis/note/EditNote";
import {CreateNote} from "../apis/note/CreateNote";
import {DeleteNote} from "../apis/note/DeleteNote";

import CreateNoteModel from "../components/models/CreateNoteModel";
import RenameNoteModel from "../components/models/RenameNoteModel";
import DeleteNoteModel from "../components/models/DeleteNoteModel";

import NoteCard from "../components/notes/NoteCard";
import NoteRow from "../components/notes/NoteRow";

import {useNavigate} from "react-router-dom";

import "./css/Home.css";

import {FiSearch} from "react-icons/fi";
import {IoSettingsSharp} from "react-icons/io5";
import {CgProfile} from "react-icons/cg";

export default function Home() {

    const [notes, setNotes] = useState([]);
    const [searchInput, setSearchInput] = useState("");
    const [search, setSearch] = useState("");
    const [sort, setSort] = useState("recent");
    const [viewMode, setViewMode] = useState("grid");

    const [openMenu, setOpenMenu] = useState(null);

    const [showModal, setShowModal] = useState(false);
    const [showRenameModal, setShowRenameModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);

    const [selectedNote, setSelectedNote] = useState(null);
    const [renameTitle, setRenameTitle] = useState("");

    const [newNote, setNewNote] = useState({
        Title: "",
        Content: "Test",
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
        .filter(note =>
            note.title?.toLowerCase().includes(search.toLowerCase())
        )
        .sort((a, b) => {

            if (sort === "Alphabetical")
                return a.title.localeCompare(b.title);

            if (sort === "recent")
                return new Date(b.lastModified) - new Date(a.lastModified);

            return 0;
        });

    async function handleCreateNote() {

        if (!newNote.Title) return;

        await CreateNote({
            Title: newNote.Title,
            Content: newNote.Content
        });

        setShowModal(false);
        setNewNote({Title: "", Content: "Test"});

        await loadNotes();
    }

    return (
        <div className="home-container">
            <div className="topbar">

                <div className="topbar-left">
                    <h2 className="app-title">Notes App</h2>
                </div>

                <div className="topbar-center">
                    <div className="search-container">

                        <input
                            className="search-bar"
                            placeholder="Search notes..."
                            value={searchInput}
                            onChange={(e) => setSearchInput(e.target.value)}
                        />

                        <button
                            className="search-btn"
                            onClick={() => setSearch(searchInput)}
                        ><FiSearch/>
                        </button>

                    </div>
                </div>

                <div className="topbar-right">

                    <div
                        className="user-info"
                        onClick={() => navigate("/profile")}
                    >
                        <span className="username">Username</span>
                        <span className="profile-icon"><CgProfile/></span>
                    </div>

                    <span className="settings-icon"
                          onClick={() => navigate("/settings")}
                    ><IoSettingsSharp/>
                    </span>

                </div>
            </div>

            <div className="content">
                <div className="toolbar">

                    <button
                        className="create-btn"
                        onClick={() => setShowModal(true)}
                    >New Note
                    </button>

                    <select className="sort"
                            value={sort}
                            onChange={(e) => setSort(e.target.value)}
                    >
                        <option value="recent">Last Edited</option>
                        <option value="Alphabetical">Alphabetical</option>
                    </select>

                    <div className="view-toggle">

                        <button
                            className={viewMode === "grid" ? "active" : ""}
                            onClick={() => setViewMode("grid")}
                        >Grid
                        </button>

                        <button
                            className={viewMode === "list" ? "active" : ""}
                            onClick={() => setViewMode("list")}
                        >List
                        </button>

                    </div>
                </div>

                <div className={viewMode === "grid" ? "notes-grid" : "notes-list"}>

                    {filteredNotes.map(note => {

                        const props = {
                            note,
                            openMenu,
                            setOpenMenu,
                            onOpen: () => navigate(`/note/${note.noteId}`),
                            onRename: () => {
                                setSelectedNote(note);
                                setRenameTitle(note.title);
                                setShowRenameModal(true);
                                setOpenMenu(null);
                            },
                            onDelete: () => {
                                setSelectedNote(note);
                                setShowDeleteModal(true);
                                setOpenMenu(null);
                            }
                        };

                        return viewMode === "grid"
                            ? <NoteCard key={note.noteId} {...props}/>
                            : <NoteRow key={note.noteId} {...props}/>

                    })}
                </div>
            </div>

            <CreateNoteModel
                show={showModal}
                newNote={newNote}
                setNewNote={setNewNote}
                onCancel={() => setShowModal(false)}
                onCreate={handleCreateNote}
            />

            <RenameNoteModel
                show={showRenameModal}
                renameTitle={renameTitle}
                setRenameTitle={setRenameTitle}
                onCancel={() => setShowRenameModal(false)}
                onSave={async () => {
                    await EditNote(selectedNote.noteId, renameTitle);
                    await loadNotes();
                    setShowRenameModal(false);
                }}
            />

            <DeleteNoteModel
                show={showDeleteModal}
                note={selectedNote}
                onCancel={() => setShowDeleteModal(false)}
                onConfirm={async () => {
                    await DeleteNote(selectedNote.noteId);
                    await loadNotes();
                    setSelectedNote(null);
                    setShowDeleteModal(false);
                }}
            />

        </div>
    );
}