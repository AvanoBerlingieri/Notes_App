import {useState} from "react";
import {loginUser} from "../apis/auth/Login.js";
import { Link, useNavigate } from "react-router-dom";
import "./css/Login.css"

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
        <div className="login-container">
            <div className="login-card">
                <h2>Login</h2>

                <form onSubmit={handleSubmit}>
                    <input
                        name="usernameOrEmail"
                        placeholder="Enter username or email"
                        onChange={handleChange}
                        required={true}
                    />
                    <input
                        name="password"
                        placeholder="Password"
                        type="password"
                        onChange={handleChange}
                        required={true}
                    />
                    <button className={"loginBtn"} type="submit">Login</button>
                </form>

                <div className="signup-link">
                    <p>Don't have an account?</p>
                    <Link to="/signup">
                        <button className={"signupBtn"}>Go to Signup</button>
                    </Link>
                </div>

                {message && <p className="message">{message}</p>}
            </div>
        </div>
    );
}
