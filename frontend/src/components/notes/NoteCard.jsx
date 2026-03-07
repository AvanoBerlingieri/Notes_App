import {FiMoreVertical} from "react-icons/fi";

// Displays a note in card format (grid view)
export default function NoteCard({note, openMenu, setOpenMenu, onOpen, onRename, onDelete}) {

    // Determine preview text
    let previewText = "No content...";

    if (note.content) {
        previewText = note.content.substring(0, 40);
    }

    // Determine if menu should be visible
    let showMenu = false;

    if (openMenu === note.noteId) {
        showMenu = true;
    }

    return (
        <div className="note-card"
             onClick={onOpen}
        >

            <div className="note-content">

                <h3>{note.title}</h3>

                <p className="preview">{previewText}</p>

            </div>

            <div className="note-footer">

                <span className="date">
                    Last modified: {new Date(note.lastModified).toLocaleString()}
                </span>

                <div className="note-menu"
                     onClick={(e) => e.stopPropagation()}
                >

                    <button className="menu-btn"
                            onClick={() => {
                                if (openMenu === note.noteId) {
                                    setOpenMenu(null);
                                } else {
                                    setOpenMenu(note.noteId);
                                }
                            }}
                    ><FiMoreVertical/>
                    </button>

                    {showMenu && (

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