import {useState} from "react";
import {signupUser} from "../apis/auth/Signup";
import {getPasswordStrength, getPasswordRules, isPasswordValid} from "../components/password/PasswordFunctions"
import {Link, Navigate, useNavigate} from "react-router-dom";
import {FaEye, FaEyeSlash} from "react-icons/fa";
import "./css/Signup.css";
import {useAuth} from "../context/AuthContext";

export default function Signup() {
    // State to hold form input values
    const [form, setForm] = useState({
        username: "",
        email: "",
        password: "",
        confirmPassword: ""
    });

    const navigate = useNavigate();
    const {authenticated, loading} = useAuth();

    const [message, setMessage] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [load, setLoad] = useState(false);

    // Password strength
    const passwordStrength = getPasswordStrength(form.password);
    const passwordRules = getPasswordRules(form.password);
    const passwordsMatch = form.password === form.confirmPassword;

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const validFormat = emailRegex.test(form.email);
    const lengthReq = form.username.length >= 5 && form.email.length > 0
        && form.password.length >= 8 && form.confirmPassword.length >= 8

    const isValid = validFormat && lengthReq;

    // Handle input changes
    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value});
        setMessage("");
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoad(true);

        if (!form.username || !form.email || !form.password) {
            setMessage("Please Fill In All Required Fields");
            return;
        }

        if (!isPasswordValid(form.password)) {
            setMessage(
                "Password must be at least 8 characters, include a number and special character"
            );
            return;
        }

        if (!passwordsMatch) {
            setMessage("Passwords do not match");
            return;
        }

        try {
            const res = await signupUser(form);
            setMessage(res.message);
            navigate("/");
        } catch (err) {
            setMessage(err.response?.data?.message || "Error signing up");
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
                <h2>Signup</h2>
                <p className="signup-subtitle">Create your account to start organizing notes</p>

                <form onSubmit={handleSubmit}>
                    <div className="input-group">
                        <span className="required">*</span>
                        <input
                            name="username"
                            placeholder="Username"
                            value={form.username}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="password-rules">
                    {form.username && (
                        <p className={form.username.length >= 5 ? "valid" : "invalid"}>
                            Username must be at least 5 characters
                        </p>
                    )}
                    </div>

                    <div className="input-group">
                        <span className="required">*</span>
                        <input
                            name="email"
                            placeholder="Email"
                            type="email"
                            value={form.email}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    <div className="input-group password-group">
                        <span className="required">*</span>

                        <input
                            name="password"
                            placeholder="Password"
                            type={showPassword ? "text" : "password"}
                            onChange={handleChange}
                            required
                        />

                        <span className="eye-icon"
                              onClick={() => setShowPassword(!showPassword)}
                        >{showPassword ? <FaEyeSlash/> : <FaEye/>}
                        </span>

                    </div>

                    {form.password && (
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

                    {form.password && (
                        <div className="password-strength">
                            <div className={`strength-bar ${passwordStrength.class}`}></div>
                            <span>{passwordStrength.label}</span>
                        </div>
                    )}

                    <div className="input-group">
                        <span className="required">*</span>
                        <input
                            name="confirmPassword"
                            placeholder="Confirm Password"
                            type="password"
                            value={form.confirmPassword}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    {!passwordsMatch && form.confirmPassword && (
                        <p className="password-error">Passwords do not match</p>
                    )}

                    <button className="signupBtn"
                            type="submit"
                            disabled={load || !passwordsMatch || !isValid}
                    >
                        {load ? "Creating Account..." : "Sign Up"}
                    </button>

                </form>

                <div className="login-link">
                    <p>Already have an account?</p>
                    <Link to="/">
                        <button className="backBtn">Back to Login</button>
                    </Link>
                </div>

                {message && <p className="message">{message}</p>}
            </div>
        </div>
    );
}