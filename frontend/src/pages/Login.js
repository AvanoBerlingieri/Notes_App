import {useState} from "react";
import {loginUser} from "../apis/auth/Login.js";
import {Link, Navigate, useNavigate} from "react-router-dom";
import {useAuth} from "../context/AuthContext";
import "./css/Signup.css"
import {FaEye, FaEyeSlash} from "react-icons/fa";
import {GetUser} from "../apis/auth/GetUser";

export default function Login() {
    // State to hold form input values
    const [form, setForm] = useState({
        usernameOrEmail: "",
        password: ""
    });

    const {authenticated, loading, setUser, setAuthenticated} = useAuth();

    // hooks for navigation
    const navigate = useNavigate();

    // to set messages
    const [message, setMessage] = useState("");

    const [showPassword, setShowPassword] = useState(false);
    const [load, setLoad] = useState(false);

    // Handle changes in input fields
    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value});
        setMessage("");
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoad(true);

        // client side validation
        if (!form.usernameOrEmail || !form.password) {
            setMessage("Please fill in all fields");
            setLoad(false);
            return;
        }

        // api call to login user
        try {
            const res = await loginUser(form);

            const user = await GetUser();
            setUser(user);
            setAuthenticated(true);
            setMessage(res.message);

            navigate("/home");

        } catch (err) {
            setMessage(err.response?.data?.message || "Invalid username/email or password");
            setLoad(false);
        }
    };

    if (loading) return null;

    if (authenticated) {
        return <Navigate to="/home" replace/>;
    }

    return (
        <div className="signup-container">
            <div className="signup-card">
                <h2>Login</h2>
                <p className="signup-subtitle">
                    Welcome back! Login to continue organizing your notes
                </p>

                <form onSubmit={handleSubmit}>

                    <div className="input-group">

                        <input className="input-login"
                               name="usernameOrEmail"
                               placeholder="Username or Email"
                               value={form.usernameOrEmail}
                               onChange={handleChange}
                               required
                        />
                    </div>

                    <div className="input-group password-group">

                        <input className="input-login"
                               name="password"
                               placeholder="Password"
                               type={showPassword ? "text" : "password"}
                               value={form.password}
                               onChange={handleChange}
                               required
                        />

                        <span
                            className="eye-icon"
                            onClick={() => setShowPassword(!showPassword)}
                        >
                        {showPassword ? <FaEyeSlash/> : <FaEye/>}
                    </span>
                    </div>

                    <button className="signupBtn"
                            type="submit"
                            disabled={load || !form.usernameOrEmail || !form.password}
                    >{load ? "Logging in..." : "Login"}
                    </button>

                </form>

                <div className="login-link">
                    <p>Don't have an account?</p>
                    <Link to="/signup">
                        <button className="backBtn">Go to Signup</button>
                    </Link>
                </div>

                {message && <p className="message">{message}</p>}
            </div>
        </div>
    );
}
