import type { Booking, BookingRequest } from '../types/Booking'
import { apiRequest } from './apiClient'

const bookingsUrl = '/api/bookings'

export async function getBookings(): Promise<Booking[]> {
    const response = await apiRequest(
        bookingsUrl,
        'Kunne ikke hente bookinger.'
    )

    return response.json()
}

/** Alle kommende økter, også andres, slik at man kan bli med. */
export async function getOpenSessions(): Promise<Booking[]> {
    const response = await apiRequest(
        `${bookingsUrl}/open`,
        'Kunne ikke hente økter.'
    )

    return response.json()
}

export async function createBooking(
    booking: BookingRequest
): Promise<Booking> {
    const response = await apiRequest(
        bookingsUrl,
        'Kunne ikke lagre bookingen.',
        {
            method: 'POST',
            body: JSON.stringify(booking),
        }
    )

    return response.json()
}

export async function updateBooking(
    id: number,
    booking: BookingRequest
): Promise<void> {
    await apiRequest(
        `${bookingsUrl}/${id}`,
        'Kunne ikke lagre bookingen.',
        {
            method: 'PUT',
            body: JSON.stringify(booking),
        }
    )
}

export async function deleteBooking(id: number): Promise<void> {
    await apiRequest(
        `${bookingsUrl}/${id}`,
        'Kunne ikke slette bookingen.',
        {
            method: 'DELETE',
        }
    )
}

export async function joinSession(id: number): Promise<void> {
    await apiRequest(
        `${bookingsUrl}/${id}/participants`,
        'Kunne ikke bli med i økten.',
        {
            method: 'POST',
        }
    )
}

export async function leaveSession(id: number): Promise<void> {
    await apiRequest(
        `${bookingsUrl}/${id}/participants`,
        'Kunne ikke forlate økten.',
        {
            method: 'DELETE',
        }
    )
}
