import { createContext, useContext } from 'react'
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

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)

/** Gives components access to the current login and authentication actions. */
export function useAuth(): AuthContextValue {
    const context = useContext(AuthContext)
    if (!context) throw new Error('useAuth must be used within an AuthProvider')
    return context
}
