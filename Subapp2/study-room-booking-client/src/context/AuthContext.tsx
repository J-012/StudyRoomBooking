import { useState } from 'react'
import type { ReactNode } from 'react'
import type { AuthResponse } from '../types/Auth'
import { AuthContext } from './useAuth'
import { readStoredAuth, storeAuth, clearStoredAuth } from '../utils/authStorage'

/** Keeps the current login in React state and persistent storage in sync. */
export function AuthProvider({ children }: { children: ReactNode }) {
    const [auth, setAuth] = useState<AuthResponse | null>(readStoredAuth)

    /** Stores the new session after a successful login or registration. */
    function login(newAuth: AuthResponse) {
        storeAuth(newAuth)
        setAuth(newAuth)
    }

    /** Clears the session and removes access to protected UI routes. */
    function logout() {
        clearStoredAuth()
        setAuth(null)
    }

    return (
        <AuthContext.Provider value={{
            token: auth?.token ?? null,
            email: auth?.email ?? null,
            fullName: auth?.fullName ?? null,
            role: auth?.role ?? null,
            isAuthenticated: auth !== null,
            isAdmin: auth?.role === 'Admin',
            login,
            logout,
        }}>
            {children}
        </AuthContext.Provider>
    )
}
