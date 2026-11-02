import type { ReactNode } from 'react'
import { Navigate } from 'react-router-dom'

import { useAuth } from '../context/AuthContext'

interface ProtectedRouteProps {
    children: ReactNode
    adminOnly?: boolean
}

function ProtectedRoute({
    children,
    adminOnly = false,
}: ProtectedRouteProps) {
    const {
        isAuthenticated,
        isAdmin,
    } = useAuth()

    if (!isAuthenticated) {
        return (
            <Navigate
                to="/login"
                replace
            />
        )
    }

    if (adminOnly && !isAdmin) {
        return (
            <Navigate
                to="/"
                replace
            />
        )
    }

    return <>{children}</>
}

export default ProtectedRoute