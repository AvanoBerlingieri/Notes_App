import Modal from "../Modal";
import "../Modal.css";

export default function ChangePasswordModal({show, password, setPassword, onCancel, onSave}) {

    // Password strength calculation
    function getPasswordStrength(pass) {
        let score = 0;
        if (pass.length >= 8) score++;
        if (/[0-9]/.test(pass)) score++;
        if (/[!@#$%^&*(),.?":{}|<>]/.test(pass)) score++;
        if (/[A-Z]/.test(pass)) score++;
        if (score <= 1) return {label: "Weak", class: "weak"};
        if (score === 2 || score === 3) return {label: "Medium", class: "medium"};
        return {label: "Strong", class: "strong"};
    }

    const strength = getPasswordStrength(password?.newPassword || "");

    // Validation
    const passwordRegex = /^(?=.*[0-9])(?=.*[!@#$%^&*(),.?":{}|<>]).{8,}$/;
    const passwordsMatch = (password?.newPassword || "") === (password?.confirmPassword || "");
    const isValid =
        passwordRegex.test(password?.newPassword || "") &&
        passwordsMatch &&
        (password?.currentPassword || "");

    return (
        <Modal
            show={show}
            title="Change Password"
            onCancel={onCancel}
            onConfirm={isValid ? onSave : () => {
            }}
            confirmText="Save"
            confirmClass={`create-btn ${!isValid ? "disabled" : ""}`} // add disabled class when invalid
        >
            <input
                type="password"
                placeholder="Enter current password"
                value={password.currentPassword}
                onChange={e => setPassword({...password, currentPassword: e.target.value})}
            />

            <input
                type="password"
                placeholder="New password"
                value={password.newPassword}
                onChange={e => setPassword({...password, newPassword: e.target.value})}
            />

            {password.newPassword && (
                <div className="password-strength">
                    <div className={`strength-bar ${strength.class}`}></div>
                    <span>{strength.label}</span>
                </div>
            )}

            <input
                type="password"
                placeholder="Re-enter new password"
                value={password.confirmPassword}
                onChange={e => setPassword({...password, confirmPassword: e.target.value})}
            />

            {!passwordsMatch && password.confirmPassword && (
                <p className="password-error">Passwords do not match</p>
            )}

            <p className="password-hint">
                Must be 8+ characters, include a number and special character.
            </p>

        </Modal>
    );
}