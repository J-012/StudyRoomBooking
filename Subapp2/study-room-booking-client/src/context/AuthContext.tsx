import {
    createContext,
    useContext,
    useState,
} from 'react'
import type { ReactNode } from 'react'

import type { AuthResponse } from '../types/Auth'

interface AuthContextValue {
    token: string | null
    email: string | null
    fullName: string | null
    role: string | null
    isAuthenticated: boolean
    isAdmin: boolean
    login: (auth: AuthResponse) => void
    logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(
    undefined
)

function readStoredAuth(): AuthResponse | null {
    const raw = localStorage.getItem('auth')

    if (!raw) {
        return null
    }

    try {
        return JSON.parse(raw) as AuthResponse
    } catch {
        return null
    }
}

export function clearStoredAuth() {
    localStorage.removeItem('auth')
}

export function AuthProvider({ children }: { children: ReactNode }) {
    const [auth, setAuth] = useState<AuthResponse | null>(
        readStoredAuth()
    )

    function login(newAuth: AuthResponse) {
        localStorage.setItem('auth', JSON.stringify(newAuth))
        setAuth(newAuth)
    }

    function logout() {
        clearStoredAuth()
        setAuth(null)
    }

    const value: AuthContextValue = {
        token: auth?.token ?? null,
        email: auth?.email ?? null,
        fullName: auth?.fullName ?? null,
        role: auth?.role ?? null,
        isAuthenticated: auth !== null,
        isAdmin: auth?.role === 'Admin',
        login,
        logout,
    }

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth() {
    const context = useContext(AuthContext)

    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider')
    }

    return context
}