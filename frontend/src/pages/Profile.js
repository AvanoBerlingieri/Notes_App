import "./css/Profile.css";
import Topbar from "../components/layout/Topbar";

import ChangeEmailModal from "../components/modals/profile/ChangeEmailModal";
import ChangePasswordModal from "../components/modals/profile/ChangePasswordModal";
import ChangeNameModal from "../components/modals/profile/ChangeNameModal";

import { useState } from "react";

export default function Profile() {
    const [user, setUser] = useState({
        username: "avano",
        firstName: "Avano",
        lastName: "Berlingieri",
        email: "avano@example.com"
    });

    const [showEmailModal, setShowEmailModal] = useState(false);
    const [showNameModal, setShowNameModal] = useState(false);
    const [showPasswordModal, setShowPasswordModal] = useState(false);

    const [email, setEmail] = useState("");
    const [name, setName] = useState({ firstName: "", lastName: "" });

    const [password, setPassword] = useState({
        currentPassword: "",
        newPassword: "",
        confirmPassword: ""
    });

    return (
        <div className="profile-page">

            <Topbar
                title="Profile"
                username={user.username}
                showBack={true}
                saveStatus={false}
                showSearch={false}
            />

            <div className="profile-content">
                <div className="profile-card">

                    <h2 className="profile-username">{user.username}</h2>

                    <div className="profile-field">
                        <div>
                            <span className="field-label">Full Name</span>
                            <p>{user.firstName} {user.lastName}</p>
                        </div>
                        <button
                            className="edit-btn"
                            onClick={() => {
                                setName({ firstName: user.firstName, lastName: user.lastName });
                                setShowNameModal(true);
                            }}
                        >
                            Change
                        </button>
                    </div>

                    <div className="profile-field">
                        <div>
                            <span className="field-label">Email</span>
                            <p>{user.email}</p>
                        </div>
                        <button
                            className="edit-btn"
                            onClick={() => {
                                setEmail(user.email);
                                setShowEmailModal(true);
                            }}
                        >
                            Change
                        </button>
                    </div>

                    <div className="profile-field">
                        <div>
                            <span className="field-label">Password</span>
                            <p>**********</p>
                        </div>
                        <button
                            className="edit-btn"
                            onClick={() => setShowPasswordModal(true)}
                        >
                            Change
                        </button>
                    </div>

                </div>
            </div>

            <ChangeNameModal
                show={showNameModal}
                name={name}
                setName={setName}
                onCancel={() => setShowNameModal(false)}
                onSave={() => {
                    console.log("Update name: ", name);
                    setShowNameModal(false);
                }}
            />

            <ChangeEmailModal
                show={showEmailModal}
                email={email}
                setEmail={setEmail}
                onCancel={() => setShowEmailModal(false)}
                onSave={() => {
                    console.log("Update email: ", email);
                    setShowEmailModal(false);
                }}
            />

            <ChangePasswordModal
                show={showPasswordModal}
                password={password}
                setPassword={setPassword}
                onCancel={() => setShowPasswordModal(false)}
                onSave={() => {
                    console.log("Change password: ", password);

                    // Reset inputs after saving
                    setPassword({ currentPassword: "", newPassword: "", confirmPassword: "" });
                    setShowPasswordModal(false);
                }}
            />

        </div>
    );
}