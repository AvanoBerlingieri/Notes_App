import Modal from "../Modal";

export default function ChangeNameModal({
                                            show, name,
                                            setName, onCancel,
                                            onSave
                                        }) {

    return (
        <Modal
            show={show}
            title="Change Name"
            onCancel={onCancel}
            onConfirm={onSave}
            confirmText="Save"
        >
            <input
                placeholder="First name"
                value={name.firstName}
                onChange={(e) =>
                    setName({...name, firstName: e.target.value})
                }
            />

            <input
                placeholder="Last name"
                value={name.lastName}
                onChange={(e) =>
                    setName({...name, lastName: e.target.value})
                }
            />
        </Modal>
    );
}