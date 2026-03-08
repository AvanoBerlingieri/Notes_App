import {useState} from "react";
import {signupUser} from "../apis/auth/Signup";
import {Link, useNavigate} from "react-router-dom";
import {FaEye, FaEyeSlash} from "react-icons/fa";
import "./css/Signup.css";

export default function Signup() {
    // State to hold form input values
    const [form, setForm] = useState({
        username: "",
        email: "",
        password: "",
        confirmPassword: ""
    });

    const navigate = useNavigate();
    const [message, setMessage] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const [loading, setLoading] = useState(false);

    // Password strength
    function getPasswordStrength(pass) {
        let score = 0;
        if (pass.length >= 8) score++;
        if (/[0-9]/.test(pass)) score++;
        if (/[!@#$%^&*(),.?":{}|<>]/.test(pass)) score++;
        if (/[A-Z]/.test(pass)) score++;

        if (score <= 1) return {label: "Weak", class: "weak"};
        if (score === 2 || score === 3) return {label: "Medium", class: "medium"};
        return {label: "Strong", class: "strong"};
    }

    const passwordStrength = getPasswordStrength(form.password);
    const passwordsMatch = form.password === form.confirmPassword;

    // Handle input changes
    const handleChange = (e) => {
        setForm({...form, [e.target.name]: e.target.value});
        setMessage("");
    };

    const passwordRules = {
        length: form.password.length >= 8,
        number: /\d/.test(form.password),
        special: /[!@#$%^&*(),.?":{}|<>]/.test(form.password)
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        const passwordRegex = /^(?=.*[0-9])(?=.*[!@#$%^&*(),.?":{}|<>]).{8,}$/;

        if (!form.username || !form.email || !form.password) {
            setMessage("Please Fill In All Required Fields");
            return;
        }

        if (!passwordRegex.test(form.password)) {
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
            setLoading(false);
        }
    };

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
                            disabled={loading || !form.password || !passwordsMatch}
                    >
                        {loading ? "Creating Account..." : "Sign Up"}
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