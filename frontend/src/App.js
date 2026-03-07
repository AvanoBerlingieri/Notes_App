import {BrowserRouter, Routes, Route} from "react-router-dom";
import Login from "./pages/Login";
import Signup from "./pages/Signup";
import Home from "./pages/Home";
import Profile from "./pages/Profile";
import NoteEditor from "./pages/NoteEditor";

export default function App() {
  return (
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<Login/>}/>
            <Route path="/signup" element={<Signup/>}/>
            <Route path="/home" element={<Home/>}/>
            <Route path="/profile" element={<Profile/>}/>
            <Route path="/note/:noteId" element={<NoteEditor/>}/>          </Routes>
        </BrowserRouter>
  );
}
