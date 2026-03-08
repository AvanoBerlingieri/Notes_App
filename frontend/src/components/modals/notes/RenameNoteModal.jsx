import Modal from "../Modal";

export default function RenameNoteModal({
                                            show, renameTitle,
                                            setRenameTitle, onCancel,
                                            onSave
                                        }) {

    return (
        <Modal
            show={show}
            title="Rename Note"
            onCancel={onCancel}
            onConfirm={onSave}
            confirmText="Save"
        >
            <input
                value={renameTitle}
                onChange={(e) => setRenameTitle(e.target.value)}
            />
        </Modal>
    );
}