import "./css/Profile.css";
import Topbar from "../components/layout/Topbar";

export default function Profile() {

    // Temp user data
    const user = {
        username: "avano",
        firstName: "Avano",
        lastName: "Berlingieri",
        email: "avano@email.com"
    };

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

                        <button className="edit-btn">Change</button>

                    </div>

                    <div className="profile-field">

                        <div>
                            <span className="field-label">Email</span>
                            <p>{user.email}</p>
                        </div>

                        <button className="edit-btn">Change</button>

                    </div>

                    <div className="profile-field">

                        <div>
                            <span className="field-label">Password</span>
                            <p>**********</p>
                        </div>

                        <button className="edit-btn">Change</button>

                    </div>

                </div>
            </div>
        </div>
    );
}