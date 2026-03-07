import {FiSearch} from "react-icons/fi";
import {IoSettingsSharp} from "react-icons/io5";
import {CgProfile} from "react-icons/cg";
import {useNavigate} from "react-router-dom";
import {FaArrowLeft} from "react-icons/fa";
import "../../pages/css/Topbar.css"

export default function Topbar({
                                   title, username,
                                   showBack, saveStatus,
                                   showSearch, setSearch,
                                   searchInput, setSearchInput
                               }) {

    const navigate = useNavigate();

    return (
        <div className="topbar">

            <div className="topbar-left">
                {showBack && (
                    <button className="back-btn"
                            onClick={() => navigate("/home")}><FaArrowLeft/></button>
                )}
                <h2 className="app-title">{title}</h2>
                {saveStatus && (
                    <span className="save-indicator">{saveStatus}</span>
                )}
            </div>

            <div className="topbar-center">
                {showSearch && (

                    <div className="search-container">

                        <input className="search-bar"
                               placeholder="Search notes..."
                               value={searchInput}
                               onChange={(e) => setSearchInput(e.target.value)}
                        />

                        <button className="search-btn"
                                onClick={() => setSearch(searchInput)}
                        ><FiSearch/>
                        </button>

                    </div>
                )}
            </div>

            <div className="topbar-right">

                <div className="user-info"
                     onClick={() => navigate("/profile")}
                >
                    <span className="username">{username}</span>

                    <span className="profile-icon"><CgProfile/></span>

                </div>

                <span className="settings-icon"
                      onClick={() => navigate("/settings")}
                ><IoSettingsSharp/>
                </span>

            </div>
        </div>
    );
}