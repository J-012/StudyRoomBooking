import { Link } from 'react-router-dom'
import { useAuth } from '../../context/useAuth'

/** Displays footer navigation for public and authenticated pages. */
export default function SiteFooter() {
    const { isAuthenticated } = useAuth()
    return (
        <footer className="footer">
            <div className="container d-flex flex-column flex-md-row justify-content-between align-items-center gap-2">
                <div>© 2026 - StudyRoomBooking</div>

                <div className="d-flex gap-4">
                    <Link to="/room-overview">Romoversikt</Link>

                    {isAuthenticated && (
                        <>
                            <Link to="/sessions">Åpne økter</Link>
                            <Link to="/bookings">Bookings</Link>
                        </>
                    )}
                </div>
            </div>
        </footer>
    )
}
