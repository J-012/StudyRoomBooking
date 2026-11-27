import { Link } from 'react-router-dom'
import { useAuth } from '../../context/useAuth'

/** Main navigation, adapted to the current user's permissions. */
export default function SiteHeader() {
    const { isAuthenticated, isAdmin, fullName, logout } = useAuth()

    return (
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
                            {/* The room overview is public */}
                            <li className="nav-item">
                                <Link
                                    className="nav-link text-secondary"
                                    to="/room-overview"
                                >
                                    Romoversikt
                                </Link>
                            </li>

                            {isAdmin && (
                                <>
                                    <li className="nav-item">
                                        <Link
                                            className="nav-link text-secondary"
                                            to="/rooms"
                                        >
                                            Rooms
                                        </Link>
                                    </li>

                                    <li className="nav-item">
                                        <Link
                                            className="nav-link text-secondary"
                                            to="/amenities"
                                        >
                                            Amenities
                                        </Link>
                                    </li>

                                    <li className="nav-item">
                                        <Link
                                            className="nav-link text-secondary"
                                            to="/buildings"
                                        >
                                            Buildings
                                        </Link>
                                    </li>
                                </>
                            )}

                            {/* Study sessions and bookings require authentication */}
                            {isAuthenticated && (
                                <>
                                    <li className="nav-item">
                                        <Link
                                            className="nav-link text-secondary"
                                            to="/sessions"
                                        >
                                            Åpne økter
                                        </Link>
                                    </li>

                                    <li className="nav-item">
                                        <Link
                                            className="nav-link text-secondary"
                                            to="/bookings"
                                        >
                                            Bookings
                                        </Link>
                                    </li>
                                </>
                            )}

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
                                    <li className="nav-item mt-2 mt-lg-0">
                                        <Link
                                            className="btn btn-outline-dark px-3"
                                            to="/login"
                                        >
                                            Log in
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
    )
}
