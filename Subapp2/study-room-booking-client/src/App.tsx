import RoomsPage from './pages/RoomsPage'
import './App.css'

function App() {
    return (
        <div className="app-shell">

            <header className="bg-white border-bottom sticky-top">
                <nav className="navbar navbar-expand-lg navbar-light py-3">
                    <div className="container">

                        <a className="navbar-brand fw-bold text-dark" href="/">
                            StudyRoomBooking
                        </a>

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
                                    <a
                                        className="nav-link text-secondary"
                                        href="/"
                                    >
                                        Home
                                    </a>
                                </li>

                                <li className="nav-item">
                                    <a
                                        className="nav-link text-secondary"
                                        href="/"
                                    >
                                        Rooms
                                    </a>
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

            <RoomsPage />

            <footer className="footer">
                <div className="container d-flex flex-column flex-md-row justify-content-between align-items-center gap-2">

                    <div>
                        © 2026 - StudyRoomBooking
                    </div>

                    <div className="d-flex gap-4">
                        <a href="/">Rooms</a>
                        <a href="/">Bookings</a>
                        <a href="/">Privacy</a>
                    </div>

                </div>
            </footer>

        </div>
    )
}

export default App