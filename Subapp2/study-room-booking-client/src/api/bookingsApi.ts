import type { Booking, BookingRequest } from '../types/Booking'
import type { Room } from '../types/Room'

import { clearStoredAuth } from '../context/AuthContext'

const bookingsUrl = '/api/bookings'
const roomsUrl = '/api/rooms'

function authHeaders(): HeadersInit {
    const raw = localStorage.getItem('auth')
    const token = raw ? JSON.parse(raw).token : null

    return {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
    }
}

function handleUnauthorized(response: Response) {
    if (response.status === 401) {
        clearStoredAuth()
        window.location.href = '/login'
    }
}

async function getErrorMessage(response: Response) {
    try {
        const data = await response.json()
        if (data.message) {
            return data.message
        }
    } catch {
        // Svaret inneholdt ikke JSON.
    }

    if (response.status === 403) {
        return 'Du har ikke tilgang til å endre denne bookingen.'
    }

    if (response.status === 409) {
        return 'Rommet er allerede booket i dette tidsrommet.'
    }

    return 'Noe gikk galt.'
}

async function ensureOk(response: Response) {
    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(await getErrorMessage(response))
    }
}

export async function getBookings(): Promise<Booking[]> {
    const response = await fetch(bookingsUrl, { headers: authHeaders() })
    await ensureOk(response)
    return response.json()
}

export async function createBooking(booking: BookingRequest): Promise<Booking> {
    const response = await fetch(bookingsUrl, {
        method: 'POST',
        headers: authHeaders(),
        body: JSON.stringify(booking),
    })
    await ensureOk(response)
    return response.json()
}

export async function updateBooking(
    id: number,
    booking: BookingRequest
): Promise<void> {
    const response = await fetch(`${bookingsUrl}/${id}`, {
        method: 'PUT',
        headers: authHeaders(),
        body: JSON.stringify(booking),
    })
    await ensureOk(response)
}

export async function deleteBooking(id: number): Promise<void> {
    const response = await fetch(`${bookingsUrl}/${id}`, {
        method: 'DELETE',
        headers: authHeaders(),
    })
    await ensureOk(response)
}

/**
 * Henter rom som er ledige i hele tidsrommet.
 * excludeBookingId brukes når man endrer en booking, slik at
 * bookingens eget rom ikke regnes som opptatt av seg selv.
 */
export async function getAvailableRooms(
    startTime: string,
    endTime: string,
    excludeBookingId?: number
): Promise<Room[]> {
    const params = new URLSearchParams({ startTime, endTime })

    if (excludeBookingId !== undefined) {
        params.set('excludeBookingId', String(excludeBookingId))
    }

    const response = await fetch(`${roomsUrl}/available?${params}`, {
        headers: authHeaders(),
    })
    await ensureOk(response)
    return response.json()
}