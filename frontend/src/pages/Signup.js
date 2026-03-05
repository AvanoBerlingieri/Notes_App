import {useState} from "react";
import {signupUser} from "../apis/Signup";
import { Link, useNavigate } from "react-router-dom";
import "./css/Signup.css";

export default function Signup() {
    // State to hold form input values
    const [form, setForm] = useState({
        username: "",
        email: "",
        password: "",
        firstname:"",
        lastname:""
    });

    // nav hooks
    const navigate = useNavigate();

    // to set messages
    const [message, setMessage] = useState("");

    //handle input change and update state
    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value});
        setMessage("");
    };

    // handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();

        // client side validation
        if (!form.username || !form.email || !form.password) {
            setMessage("Please Fill In All Fields");
            return;
        }

        // api call to create new user
        try {
            const res = await signupUser(form);

            setMessage(res.message);
            navigate("/");

        } catch (err) {
            setMessage(err.response?.data?.message || "Error signing up");
        }
    };

    return (
        <div className="signup-container">
            <div className="signup-card">
                <h2>Signup</h2>

                <form onSubmit={handleSubmit}>
                    <input
                        name="username"
                        placeholder="Username"
                        onChange={handleChange}
                        required={true}
                    />
                    <input
                        name="email"
                        placeholder="Email"
                        type="email"
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
                    <input
                        name="firstname"
                        placeholder="First Name"
                        onChange={handleChange}
                        required={false}
                    />
                    <input
                        name="lastname"
                        placeholder="Last Name"
                        onChange={handleChange}
                        required={false}
                    />
                    <button className={"signupBtn"} type="submit">Sign Up</button>
                </form>

                <div className="login-link">
                    <p>Already have an account?</p>
                    <Link to="/">
                        <button className={"backBtn"}>Back to Login</button>
                    </Link>
                </div>

                {message && <p className="message">{message}</p>}
            </div>
        </div>
    );
}
