import type {
    AuthResponse,
    LoginRequest,
    RegisterRequest,
} from '../types/Auth'

const authUrl = '/api/auth'

export async function login(
    request: LoginRequest
): Promise<AuthResponse> {
    const response = await fetch(`${authUrl}/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
    })

    if (!response.ok) {
        throw new Error('Invalid email or password')
    }

    const auth: AuthResponse = await response.json()

    return auth
}

export async function register(
    request: RegisterRequest
): Promise<AuthResponse> {
    const response = await fetch(`${authUrl}/register`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
    })

    if (!response.ok) {
        const problem = await response.json().catch(() => null)
        const message =
            problem?.errors?.[''] ?? problem?.title ?? 'Kunne ikke registrere bruker.'

        throw new Error(Array.isArray(message) ? message.join(' ') : message)
    }

    const auth: AuthResponse = await response.json()

    return auth
}