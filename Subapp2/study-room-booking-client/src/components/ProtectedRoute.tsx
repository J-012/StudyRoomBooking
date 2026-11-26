import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from '../context/useAuth'

interface ProtectedRouteProps {
    children: ReactNode
    adminOnly?: boolean
}

/** Redirects unauthorized visitors. The API independently enforces authorization. */
export default function ProtectedRoute({ children, adminOnly = false }: ProtectedRouteProps) {
    const { isAuthenticated, isAdmin } = useAuth()

    if (!isAuthenticated) return <Navigate to="/login" replace />
    if (adminOnly && !isAdmin) return <Navigate to="/" replace />

    return <>{children}</>
}
