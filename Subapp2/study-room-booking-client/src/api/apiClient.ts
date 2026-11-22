import { clearStoredAuth } from '../context/AuthContext'

// Shared HTTP helper for the API modules: adds the JWT, sends the user to
// the login page when the token is rejected, and turns error responses
// into an Error carrying the message from the server.

function authHeaders(): HeadersInit {
    let token: string | null = null

    try {
        const raw = localStorage.getItem('auth')
        token = raw ? JSON.parse(raw).token : null
    } catch {
        // Corrupt value in localStorage: treat as not logged in.
    }

    return {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
    }
}

async function getErrorMessage(response: Response, fallback: string) {
    try {
        const data = await response.json()

        if (typeof data?.message === 'string') {
            return data.message
        }
    } catch {
        // Response had no JSON body.
    }

    return fallback
}

/**
 * Sends a request to the API and returns the response if it succeeded.
 * Throws an Error with the server's message (or fallbackMessage) otherwise.
 */
export async function apiRequest(
    url: string,
    fallbackMessage: string,
    init: RequestInit = {}
): Promise<Response> {
    const response = await fetch(url, {
        ...init,
        headers: authHeaders(),
    })

    if (response.status === 401) {
        clearStoredAuth()
        window.location.href = '/login'
    }

    if (!response.ok) {
        throw new Error(await getErrorMessage(response, fallbackMessage))
    }

    return response
}
