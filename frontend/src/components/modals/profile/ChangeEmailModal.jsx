import Modal from "../Modal";

export default function ChangeEmailModal({
                                             show, email,
                                             setEmail, onCancel,
                                             onSave
                                         }) {

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    const emailMatched = email.newEmail === email.confirmEmail;
    const notEmpty = email.newEmail.length > 0;
    const validFormat = emailRegex.test(email.newEmail);

    const isValid = emailMatched && validFormat && notEmpty;

    return (
        <Modal
            show={show}
            title="Change Email"
            onCancel={onCancel}
            onConfirm={isValid ? onSave : () => {
            }}
            confirmText="Save"
            confirmClass={`create-btn ${!isValid ? "disabled" : ""}`}
        >
            <div className="input-group">
                <input
                    placeholder="Current email"
                    value={email.currentEmail}
                    type="email"
                    onChange={(e) =>
                        setEmail({...email, currentEmail: e.target.value})
                    }
                />
            </div>

            <div className="input-group">
                <input
                    placeholder="New email"
                    value={email.newEmail}
                    type="email"
                    onChange={(e) =>
                        setEmail({...email, newEmail: e.target.value})
                    }
                />
            </div>

            <div className="input-group">
                <input
                    placeholder="Re-enter email"
                    value={email.confirmEmail}
                    type="email"
                    onChange={(e) =>
                        setEmail({...email, confirmEmail: e.target.value})
                    }
                />
            </div>

            {!emailMatched && email.confirmEmail && (
                <p className="password-error">
                    Email does not match
                </p>
            )}

        </Modal>
    );
}