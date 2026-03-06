export default function DeleteNoteModel({show, note, onCancel, onConfirm}) {

    if (!show) return null;

    return (
        <div className="modal-overlay">
            <div className="modal">

                <h2>Delete {note?.title}</h2>

                <p>
                    Are you sure you want to delete this note?
                </p>

                <div className="modal-buttons">

                    <button
                        className="cancel-btn"
                        onClick={onCancel}
                    >
                        Cancel
                    </button>

                    <button
                        className="delete-btn"
                        onClick={onConfirm}
                    >
                        Confirm
                    </button>

                </div>

            </div>
        </div>
    );
}