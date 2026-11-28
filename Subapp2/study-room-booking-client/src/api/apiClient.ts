import { clearStoredAuth, readStoredAuth } from '../utils/authStorage'

/** Extracts both custom errors and ASP.NET validation problem details. */
async function getErrorMessage(response: Response, fallback: string): Promise<string> {
    try {
        const body: unknown = await response.json()
        if (typeof body === 'string' && body.trim()) return body
        if (typeof body !== 'object' || body === null) return fallback
        const data = body as Record<string, unknown>
        if (typeof data.message === 'string' && data.message) return data.message

        if (data.errors && typeof data.errors === 'object') {
            const messages = Object.values(data.errors).flatMap(value =>
                Array.isArray(value) ? value.filter(item => typeof item === 'string') : []
            )
            if (messages.length) return messages.join(' ')
        }

        if (typeof data.detail === 'string' && data.detail) return data.detail
        if (typeof data.title === 'string' && data.title) return data.title
    } catch {
        // Proxy failures and empty responses may not contain JSON.
    }
    return fallback
}

/** Sends HTTP requests with consistent headers, authentication and error handling. */
export async function apiRequest(
    url: string,
    fallbackMessage: string,
    init: RequestInit = {},
    anonymous = false,
): Promise<Response> {
    const headers = new Headers(init.headers)
    if (init.body && !headers.has('Content-Type')) headers.set('Content-Type', 'application/json')

    const auth = anonymous ? null : readStoredAuth()
    if (auth) headers.set('Authorization', `Bearer ${auth.token}`)

    let response: Response
    try {
        response = await fetch(url, { ...init, headers })
    } catch {
        throw new Error('Kunne ikke koble til serveren. Prøv igjen.')
    }

    // Login failures must stay on the login form instead of reloading the page.
    if (response.status === 401 && !anonymous) {
        const current = readStoredAuth()
        // A delayed response from an old session must not log out a newer session.
        if (!current || current.token === auth?.token) {
            clearStoredAuth()
            window.location.href = '/login'
        }
        throw new Error('Logg inn på nytt for å fortsette.')
    }

    if (!response.ok) {
        const fallback = response.status === 403 ? 'Du har ikke tilgang til denne handlingen.' : fallbackMessage
        throw new Error(await getErrorMessage(response, fallback))
    }

    return response
}
