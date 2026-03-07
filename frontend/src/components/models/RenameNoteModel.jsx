export default function RenameNoteModel({show, renameTitle, setRenameTitle, onCancel, onSave}) {

    if (!show) return null;

    return (
        <div className="modal-overlay">
            <div className="modal">

                <h2>Rename Note</h2>

                <input value={renameTitle}
                       onChange={(e) => setRenameTitle(e.target.value)}
                />

                <div className="modal-buttons">

                    <button className="cancel-btn"
                            onClick={onCancel}
                    >Cancel
                    </button>

                    <button className="create-btn"
                            onClick={onSave}
                    >Save
                    </button>

                </div>
            </div>
        </div>
    );
}