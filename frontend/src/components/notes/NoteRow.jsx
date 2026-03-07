import {FiMoreVertical} from "react-icons/fi";

export default function NoteRow({note, openMenu, setOpenMenu, onOpen, onRename, onDelete}) {

    return (
        <div
            className="note-row"
            onClick={onOpen}
        >

            <div className="note-content">
                <h3>{note.title}</h3>

                <p className="preview">
                    {note.content?.substring(0, 60) || "No content..."}
                </p>
            </div>

            <div className="note-footer">

                <span className="date">
                    Last modified: {new Date(note.lastModified).toLocaleString()}
                </span>

                <div className="note-menu"
                     onClick={(e) => e.stopPropagation()}
                >

                    <button className="menu-btn"
                            onClick={() =>
                                setOpenMenu(openMenu === note.noteId ? null : note.noteId)
                            }
                    ><FiMoreVertical/>
                    </button>

                    {openMenu === note.noteId && (
                        <div className="menu-dropdown">

                            <div className="menu-item"
                                 onClick={onRename}
                            >Rename
                            </div>

                            <div className="menu-item delete"
                                 onClick={onDelete}
                            >Delete
                            </div>

                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}