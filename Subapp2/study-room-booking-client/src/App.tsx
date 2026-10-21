import { Routes, Route, Link } from 'react-router-dom'

import RoomsPage from './pages/RoomsPage'
import StudentsPage from './pages/StudentsPage'
import './App.css'

function App() {
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

                                <li className="nav-item">
                                    <Link
                                        className="nav-link text-secondary"
                                        to="/"
                                    >
                                        Rooms
                                    </Link>
                                </li>

                                <li className="nav-item">
                                    <Link
                                        className="nav-link text-secondary"
                                        to="/students"
                                    >
                                        Students
                                    </Link>
                                </li>

                                <li className="nav-item">
                                    <a
                                        className="nav-link text-secondary"
                                        href="/"
                                    >
                                        Bookings
                                    </a>
                                </li>

                                <li className="nav-item mt-2 mt-lg-0">
                                    <button className="btn btn-primary px-3">
                                        New booking
                                    </button>
                                </li>

                            </ul>
                        </div>
                    </div>
                </nav>
            </header>

            <Routes>
                <Route path="/" element={<RoomsPage />} />
                <Route path="/students" element={<StudentsPage />} />
            </Routes>

            <footer className="footer">
                <div className="container d-flex flex-column flex-md-row justify-content-between align-items-center gap-2">

                    <div>
                        © 2026 - StudyRoomBooking
                    </div>

                    <div className="d-flex gap-4">
                        <Link to="/">Rooms</Link>
                        <Link to="/students">Students</Link>
                        <a href="/">Privacy</a>
                    </div>

                </div>
            </footer>

        </div>
    )
}

export default App