import Modal from "../Modal";

export default function DeleteNoteModal({
                                            show, note,
                                            onCancel, onConfirm
                                        }) {

    return (
        <Modal
            show={show}
            title={`Delete ${note?.title}`}
            onCancel={onCancel}
            onConfirm={onConfirm}
            confirmText="Confirm"
            confirmClass="delete-btn"
        >
            <p>Are you sure you want to delete this note?</p>
        </Modal>
    );
}