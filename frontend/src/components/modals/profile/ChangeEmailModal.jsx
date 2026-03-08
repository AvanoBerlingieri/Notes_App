import Modal from "../Modal";

export default function ChangeEmailModal({
                                             show, email,
                                             setEmail, onCancel,
                                             onSave
                                         }) {

    return (
        <Modal
            show={show}
            title="Change Email"
            onCancel={onCancel}
            onConfirm={onSave}
        >
            <input
                placeholder="New email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
            />
        </Modal>
    );
}