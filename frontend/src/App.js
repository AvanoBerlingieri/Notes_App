import {BrowserRouter, Routes, Route} from "react-router-dom";
import Login from "./pages/Login";
import Signup from "./pages/Signup";
import Home from "./pages/Home";
import Profile from "./pages/Profile";
import NoteEditor from "./pages/NoteEditor";
import {AuthProvider} from "./context/AuthContext";
import ProtectedRoute from "./context/ProtectedRoute"

export default function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Login/>}/>
                    <Route path="/signup" element={<Signup/>}/>
                    <Route path="/home" element={
                        <ProtectedRoute>
                            <Home/>
                        </ProtectedRoute>
                    }/>
                    <Route path="/profile" element={
                        <ProtectedRoute>
                            <Profile/>
                        </ProtectedRoute>
                    }/>
                    <Route path="/note/:noteId" element={
                        <ProtectedRoute>
                            <NoteEditor/>
                        </ProtectedRoute>
                    }/>
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}
