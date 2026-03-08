import {useState} from "react";
import {loginUser} from "../apis/auth/Login.js";
import {Link, useNavigate} from "react-router-dom";
import "./css/Signup.css"
import {FaEye, FaEyeSlash} from "react-icons/fa";

export default function Login() {
    // State to hold form input values
    const [form, setForm] = useState({
        usernameOrEmail: "",
        password: ""
    });

    // hooks for navigation
    const navigate = useNavigate();

    // to set messages
    const [message, setMessage] = useState("");

    const [showPassword, setShowPassword] = useState(false);

    // Handle changes in input fields
    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value});
        setMessage("");
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();

        // client side validation
        if (!form.usernameOrEmail || !form.password) {
            setMessage("Please fill in all fields");
            return;
        }

        // api call to login user
        try {
            const res = await loginUser(form);

            setMessage(res.message);
            navigate("/home");

        } catch (err) {
            setMessage(err.response?.data?.message || "Login failed");
        }
    };

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

                    <button className="signupBtn" type="submit">
                        Login
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
