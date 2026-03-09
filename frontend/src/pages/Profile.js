import "./css/Profile.css";
import Topbar from "../components/layout/Topbar";

import ChangeEmailModal from "../components/modals/profile/ChangeEmailModal";
import ChangePasswordModal from "../components/modals/profile/ChangePasswordModal";
import ChangeNameModal from "../components/modals/profile/ChangeNameModal";

import {UpdateName} from "../apis/auth/UpdateName";
import {ChangePassword} from "../apis/auth/UpdatePassword";

import {useState} from "react";
import {UpdateEmail} from "../apis/auth/UpdateEmail";
import {useAuth} from "../context/AuthContext";

export default function Profile() {
    const {user, setUser} = useAuth();

    const [showEmailModal, setShowEmailModal] = useState(false);
    const [showNameModal, setShowNameModal] = useState(false);
    const [showPasswordModal, setShowPasswordModal] = useState(false);

    const [email, setEmail] = useState({
        currentEmail: "",
        newEmail: "",
        confirmEmail: "",
    });

    const [name, setName] = useState({
        firstName: "",
        lastName: ""
    });

    const [password, setPassword] = useState({
        currentPassword: "",
        newPassword: "",
        confirmPassword: ""
    });

    if (!user) return null;

    return (
        <div className="profile-page">

            <Topbar
                title="Profile"
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
                                setName({firstName: user.firstName, lastName: user.lastName});
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
                                setEmail({
                                    currentEmail: user.email,
                                    newEmail: "",
                                    confirmEmail: "",
                                })
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
                onSave={async () => {
                    try {
                        if (
                            name.firstName === user.firstName &&
                            name.lastName === user.lastName
                        ) {
                            alert("Name must be changed to save");
                            return;
                        }

                        await UpdateName({
                            firstName: name.firstName,
                            lastName: name.lastName
                        });

                        setUser({
                            ...user,
                            firstName: name.firstName,
                            lastName: name.lastName
                        });

                        setShowNameModal(false);
                    } catch (err) {
                        console.error("Failed to update name", err);
                    }
                }}
            />

            <ChangeEmailModal
                show={showEmailModal}
                email={email}
                setEmail={setEmail}
                onCancel={() => setShowEmailModal(false)}
                onSave={async () => {
                    try {

                        if (email.newEmail === user.email) {
                            alert("New email must be different to current email");
                            return;
                        }

                        await UpdateEmail({
                            currentEmail: email.currentEmail,
                            newEmail: email.newEmail
                        });

                        setUser({
                            ...user,
                            email: email.newEmail
                        });

                        setEmail({
                            currentEmail: email.newEmail,
                            newEmail: "",
                            confirmEmail: ""
                        })

                        setShowEmailModal(false);
                    } catch (err) {
                        console.error("Failed to update email", err);
                    }
                }}
            />

            <ChangePasswordModal
                show={showPasswordModal}
                password={password}
                setPassword={setPassword}
                onCancel={() => {
                    setShowPasswordModal(false)
                    setPassword({
                        currentPassword: "",
                        newPassword: "",
                        confirmPassword: ""
                    });
                }}
                onSave={async () => {
                    try {
                        await ChangePassword({
                            currentPassword: password.currentPassword,
                            newPassword: password.newPassword
                        });

                        setPassword({
                            currentPassword: "",
                            newPassword: "",
                            confirmPassword: ""
                        });

                        setShowPasswordModal(false);

                    } catch (err) {
                        console.error("Failed to change password", err);
                    }
                }}
            />

        </div>
    );
}