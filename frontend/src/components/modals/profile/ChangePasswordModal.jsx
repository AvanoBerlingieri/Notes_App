import {useState} from "react";
import {FaEye, FaEyeSlash} from "react-icons/fa";
import {getPasswordStrength, getPasswordRules, isPasswordValid} from "../../password/PasswordFunctions"
import Modal from "../Modal";
import "../Modal.css";

export default function ChangePasswordModal({
                                                show,
                                                password,
                                                setPassword,
                                                onCancel,
                                                onSave
                                            }) {

    const [showCurrent, setShowCurrent] = useState(false);
    const [showNew, setShowNew] = useState(false);
    const [showConfirm, setShowConfirm] = useState(false);
    const newPass = password?.newPassword || "";

    // Password strength
    const strength = getPasswordStrength(newPass);
    const passwordRules = getPasswordRules(newPass);
    const passwordsMatch = newPass === (password?.confirmPassword || "");

    const isValid =
        isPasswordValid(newPass) &&
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
            confirmClass={`create-btn ${!isValid ? "disabled" : ""}`}
        >

            <div className="input-group password-group">
                <input
                    type={showCurrent ? "text" : "password"}
                    placeholder="Enter current password"
                    value={password.currentPassword}
                    onChange={e =>
                        setPassword({...password, currentPassword: e.target.value})
                    }
                />

                <span
                    className="eye-icon"
                    onClick={() => setShowCurrent(!showCurrent)}
                >
                    {showCurrent ? <FaEyeSlash/> : <FaEye/>}
                </span>
            </div>

            <div className="input-group password-group">
                <input
                    type={showNew ? "text" : "password"}
                    placeholder="New password"
                    value={password.newPassword}
                    onChange={e =>
                        setPassword({...password, newPassword: e.target.value})
                    }
                />

                <span
                    className="eye-icon"
                    onClick={() => setShowNew(!showNew)}
                >
                    {showNew ? <FaEyeSlash/> : <FaEye/>}
                </span>
            </div>

            {password.newPassword && (
                <div className="password-rules">

                    <p className={passwordRules.length ? "valid" : "invalid"}>
                        At least 8 characters
                    </p>

                    <p className={passwordRules.capital ? "valid" : "invalid"}>
                        At least 1 capital letter
                    </p>

                    <p className={passwordRules.number ? "valid" : "invalid"}>
                        At least 1 number
                    </p>

                    <p className={passwordRules.special ? "valid" : "invalid"}>
                        At least 1 special character
                    </p>

                </div>
            )}

            {password.newPassword && (
                <div className="password-strength">
                    <div className={`strength-bar ${strength.class}`}></div>
                    <span>{strength.label}</span>
                </div>
            )}

            <div className="input-group password-group">
                <input
                    type={showConfirm ? "text" : "password"}
                    placeholder="Re-enter new password"
                    value={password.confirmPassword}
                    onChange={e =>
                        setPassword({
                            ...password,
                            confirmPassword: e.target.value
                        })
                    }
                />

                <span
                    className="eye-icon"
                    onClick={() => setShowConfirm(!showConfirm)}
                >
                    {showConfirm ? <FaEyeSlash/> : <FaEye/>}
                </span>
            </div>

            {!passwordsMatch && password.confirmPassword && (
                <p className="password-error">
                    Passwords do not match
                </p>
            )}

        </Modal>
    );
}