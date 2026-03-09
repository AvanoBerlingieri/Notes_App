import {useEffect, useState} from "react";
import {GetAllNotes} from "../apis/note/GetAllNotes";
import {EditNote} from "../apis/note/EditNote";
import {CreateNote} from "../apis/note/CreateNote";
import {DeleteNote} from "../apis/note/DeleteNote";

import CreateNoteModal from "../components/modals/notes/CreateNoteModal";
import RenameNoteModal from "../components/modals/notes/RenameNoteModal";
import DeleteNoteModal from "../components/modals/notes/DeleteNoteModal";

import Topbar from "../components/layout/Topbar";
import NoteCard from "../components/notes/NoteCard";
import NoteRow from "../components/notes/NoteRow";

import {useNavigate} from "react-router-dom";

import "./css/Home.css";

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

            if (sort === "alphabetical")
                return a.title.localeCompare(b.title);

            if (sort === "recent")
                return new Date(b.lastModified) - new Date(a.lastModified);

            return 0;
        });

    async function handleCreateNote() {

        if (!newNote.Title) {
            alert("Note must have title!")
            return;
        }

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

            <Topbar
                title="Notes App"
                showBack={false}
                saveStatus={false}
                showSearch={true}
                searchInput={searchInput}
                setSearchInput={setSearchInput}
                setSearch={setSearch}
            />

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
                        <option value="alphabetical">Alphabetical</option>
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

                        const noteProps = {
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
                            ? <NoteCard key={note.noteId} {...noteProps}/>
                            : <NoteRow key={note.noteId} {...noteProps}/>

                    })}
                </div>
            </div>

            <CreateNoteModal
                show={showModal}
                newNote={newNote}
                setNewNote={setNewNote}
                onCancel={() => setShowModal(false)}
                onCreate={handleCreateNote}
            />

            <RenameNoteModal
                show={showRenameModal}
                renameTitle={renameTitle}
                setRenameTitle={setRenameTitle}
                onCancel={() => setShowRenameModal(false)}
                onSave={async () => {
                    if (!renameTitle.trim()) {
                        alert("Note must have title!");
                        return;
                    }

                    if (selectedNote.title === renameTitle) {
                        alert("Note title must be different to save!");
                        return;
                    }

                    await EditNote(selectedNote.noteId, renameTitle);
                    await loadNotes();
                    setShowRenameModal(false);
                }}
            />

            <DeleteNoteModal
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