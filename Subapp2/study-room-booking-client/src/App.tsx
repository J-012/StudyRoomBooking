import { Routes, Route, Link } from 'react-router-dom'

import RoomsPage from './pages/RoomsPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ProtectedRoute from './components/ProtectedRoute'
import { useAuth } from './context/AuthContext'
import './App.css'

function App() {
    const { isAuthenticated, isAdmin, fullName, logout } = useAuth()

    return (
        <div className="app-shell">

            <header className="bg-white border-bottom sticky-top">
                <nav className="navbar navbar-expand-lg navbar-light py-3">
                    <div className="container">

                        <Link className="navbar-brand fw-bold text-dark" to="/">
                            StudyRoomBooking
                        </Link>

                        <button
                            className="navbar-toggler"
                            type="button"
                            data-bs-toggle="collapse"
                            data-bs-target="#mainNav"
                            aria-controls="mainNav"
                            aria-expanded="false"
                            aria-label="Toggle navigation"
                        >
                            <span className="navbar-toggler-icon"></span>
                        </button>

                        <div
                            className="collapse navbar-collapse"
                            id="mainNav"
                        >
                            <ul className="navbar-nav ms-auto align-items-lg-center gap-lg-2">

                                {isAdmin && (
                                    <li className="nav-item">
                                        <Link
                                            className="nav-link text-secondary"
                                            to="/"
                                        >
                                            Rooms
                                        </Link>
                                    </li>
                                )}

                                <li className="nav-item">
                                    <a
                                        className="nav-link text-secondary"
                                        href="/"
                                    >
                                        Bookings
                                    </a>
                                </li>

                                {isAuthenticated ? (
                                    <li className="nav-item mt-2 mt-lg-0 d-flex align-items-center gap-2">
                                        <span className="text-secondary small">
                                            {fullName}
                                        </span>
                                        <button
                                            type="button"
                                            className="btn btn-outline-secondary px-3"
                                            onClick={logout}
                                        >
                                            Logg ut
                                        </button>
                                    </li>
                                ) : (
                                    <>
                                        <li className="nav-item">
                                            <Link
                                                className="nav-link text-secondary"
                                                to="/login"
                                            >
                                                Logg inn
                                            </Link>
                                        </li>

                                        <li className="nav-item mt-2 mt-lg-0">
                                            <Link
                                                className="btn btn-primary px-3"
                                                to="/register"
                                            >
                                                Registrer deg
                                            </Link>
                                        </li>
                                    </>
                                )}

                            </ul>
                        </div>
                    </div>
                </nav>
            </header>

            <Routes>
                <Route
                    path="/"
                    element={
                        <ProtectedRoute>
                            <RoomsPage />
                        </ProtectedRoute>
                    }
                />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
            </Routes>

            <footer className="footer">
                <div className="container d-flex flex-column flex-md-row justify-content-between align-items-center gap-2">

                    <div>
                        © 2026 - StudyRoomBooking
                    </div>

                    <div className="d-flex gap-4">
                        <a href="/">Bookings</a>
                        <a href="/">Privacy</a>
                    </div>

                </div>
            </footer>

        </div>
    )
}

export default App