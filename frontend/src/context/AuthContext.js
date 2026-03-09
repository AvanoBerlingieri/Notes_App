import {createContext, useContext, useState, useEffect} from "react";
import {GetUser} from "../apis/auth/GetUser";

const AuthContext = createContext();

export function AuthProvider({children}) {

    const [user, setUser] = useState(null);
    const [authenticated, setAuthenticated] = useState(false);
    const [loading, setLoading] = useState(true);

    useEffect(() => {

        async function checkAuth() {
            try {
                const data = await GetUser();

                setUser(data);
                setAuthenticated(true);
            } catch (err) {
                setUser(null);
                setAuthenticated(false);
            } finally {
                setLoading(false);
            }
        }
        checkAuth();
    }, []);

    return (
        <AuthContext.Provider
            value={{
                user,
                setUser,
                authenticated,
                setAuthenticated,
                loading
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}