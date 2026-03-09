import { Navigate } from "react-router-dom";
import { useAuth } from "./AuthContext";

export default function ProtectedRoute({ children }) {

    const { authenticated, loading } = useAuth();

    if (loading) return null;

    if (!authenticated) {
        return <Navigate to="/" replace />;
    }

    return children;
}