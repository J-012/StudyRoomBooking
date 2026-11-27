import { Routes, Route, Link } from 'react-router-dom'

import HomePage from './pages/HomePage'
import RoomsPage from './pages/RoomsPage'
import BookingsPage from './pages/BookingsPage'
import SessionsPage from './pages/SessionsPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import AmenitiesPage from './pages/AmenitiesPage'
import BuildingsPage from './pages/BuildingsPage'
import ProtectedRoute from './components/ProtectedRoute'
import RoomOverviewPage from './pages/RoomOverviewPage'
import SiteHeader from './components/layout/SiteHeader'
import SiteFooter from './components/layout/SiteFooter'

/** Defines public routes and protected student/admin routes in the shared layout. */
function App() {
    return (
        <div className="app-shell">
            <SiteHeader />

            <Routes>
                <Route path="/" element={<HomePage />} />

                <Route
                    path="/rooms"
                    element={
                        <ProtectedRoute adminOnly>
                            <RoomsPage />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/amenities"
                    element={
                        <ProtectedRoute adminOnly>
                            <AmenitiesPage />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/buildings"
                    element={
                        <ProtectedRoute adminOnly>
                            <BuildingsPage />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/bookings"
                    element={
                        <ProtectedRoute>
                            <BookingsPage />
                        </ProtectedRoute>
                    }
                />

                <Route
                    path="/sessions"
                    element={
                        <ProtectedRoute>
                            <SessionsPage />
                        </ProtectedRoute>
                    }
                />

                <Route path="/room-overview" element={<RoomOverviewPage />} />

                <Route path="/login" element={<LoginPage />} />

                <Route path="/register" element={<RegisterPage />} />
                <Route path="*" element={
                    <main className="container py-5">
                        <h1 className="h3">Siden finnes ikke</h1>
                        <Link to="/">Til forsiden</Link>
                    </main>
                } />
            </Routes>

            <SiteFooter />
        </div>
    )
}

export default App
