export default function CreateNoteModel({show, newNote, setNewNote, onCancel, onCreate}) {

    if (!show) return null;

    return (
        <div className="modal-overlay">
            <div className="modal">

                <h2>Create New Note</h2>

                <input placeholder="Note title"
                       value={newNote.Title}
                       onChange={(e) =>
                           setNewNote({...newNote, Title: e.target.value})
                       }
                />

                <div className="modal-buttons">

                    <button className="cancel-btn"
                            onClick={onCancel}
                    >Cancel
                    </button>

                    <button className="create-btn"
                            onClick={onCreate}
                    >Create
                    </button>

                </div>

            </div>
        </div>
    );
}