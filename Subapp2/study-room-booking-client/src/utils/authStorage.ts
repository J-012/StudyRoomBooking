import type { AuthResponse } from '../types/Auth'

const storageKey = 'auth'

/** Reads the stored login, ignoring incomplete or expired credentials. */
export function readStoredAuth(): AuthResponse | null {
    try {
        const raw = localStorage.getItem(storageKey)
        if (!raw) return null

        const auth = JSON.parse(raw) as Partial<AuthResponse> | null
        if (!auth || typeof auth.token !== 'string' || !auth.token ||
            typeof auth.email !== 'string' || typeof auth.fullName !== 'string' ||
            typeof auth.role !== 'string') return null

        // JWT expiry is a UI convenience; signature and permissions are verified by the API.
        const payload = auth.token.split('.')[1]
        if (!payload) return null
        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
        const { exp } = JSON.parse(atob(base64)) as { exp?: number }
        if (typeof exp !== 'number' || exp * 1000 <= Date.now()) return null

        return auth as AuthResponse
    } catch {
        return null
    }
}

/** Persists the authentication response for subsequent page visits. */
export function storeAuth(auth: AuthResponse): void {
    localStorage.setItem(storageKey, JSON.stringify(auth))
}

/** Removes the locally stored authentication response. */
export function clearStoredAuth(): void {
    localStorage.removeItem(storageKey)
}
