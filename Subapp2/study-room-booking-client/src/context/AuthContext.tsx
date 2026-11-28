import { useEffect, useState } from 'react'
import type { ReactNode } from 'react'
import type { AuthResponse } from '../types/Auth'
import { AuthContext } from './useAuth'
import { authStorageKey, getTokenExpiry, readStoredAuth, storeAuth, clearStoredAuth } from '../utils/authStorage'

/** Keeps the current login in React state and persistent storage in sync. */
export function AuthProvider({ children }: { children: ReactNode }) {
    const [auth, setAuth] = useState<AuthResponse | null>(readStoredAuth)

    // Keep tabs in sync when another tab logs in or out.
    useEffect(() => {
        function syncSession(event: StorageEvent) {
            if (event.key === authStorageKey || event.key === null) setAuth(readStoredAuth())
        }
        window.addEventListener('storage', syncSession)
        return () => window.removeEventListener('storage', syncSession)
    }, [])

    // Schedule expiry outside render, and clear the timer whenever the token changes.
    const token = auth?.token
    useEffect(() => {
        if (!token) return
        const expiry = getTokenExpiry(token) ?? 0
        let timer: number

        function scheduleExpiry() {
            const remaining = expiry - Date.now()
            // Browser timers have a maximum delay of roughly 24 days.
            timer = window.setTimeout(() => {
                if (expiry > Date.now()) {
                    scheduleExpiry()
                    return
                }
                const current = readStoredAuth()
                // Do not clear a newer login written by another tab.
                if (!current) clearStoredAuth()
                setAuth(current)
            }, Math.min(Math.max(remaining, 0), 2_147_483_647))
        }

        scheduleExpiry()
        return () => window.clearTimeout(timer)
    }, [token])

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
