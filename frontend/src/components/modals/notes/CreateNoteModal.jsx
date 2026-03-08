import Modal from "../Modal";

export default function CreateNoteModal({
                                            show, newNote,
                                            setNewNote, onCancel,
                                            onCreate
                                        }) {

    return (
        <Modal
            show={show}
            title="Create New Note"
            onCancel={onCancel}
            onConfirm={onCreate}
            confirmText="Create"
        >
            <input
                placeholder="Note title"
                value={newNote.Title}
                onChange={(e) =>
                    setNewNote({...newNote, Title: e.target.value})
                }
            />
        </Modal>
    );
}