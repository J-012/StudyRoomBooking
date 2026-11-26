import type { AuthResponse, LoginRequest, RegisterRequest } from '../types/Auth'
import { apiRequest } from './apiClient'

const authUrl = '/api/auth'

/** Authenticates an existing account without attaching a previous JWT. */
export async function login(request: LoginRequest): Promise<AuthResponse> {
    const response = await apiRequest(`${authUrl}/login`, 'Feil e-post eller passord.', {
        method: 'POST',
        body: JSON.stringify(request),
    }, true)
    return response.json()
}

/** Registers a new student and returns the authentication response. */
export async function register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await apiRequest(`${authUrl}/register`, 'Kunne ikke registrere bruker.', {
        method: 'POST',
        body: JSON.stringify(request),
    }, true)
    return response.json()
}
