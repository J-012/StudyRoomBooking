import type { Booking, BookingRequest } from '../types/Booking'
import { apiRequest } from './apiClient'

const url = '/api/bookings'

/** Retrieves bookings accessible to the authenticated user. */
export async function getBookings(): Promise<Booking[]> {
    const response = await apiRequest(url, 'Kunne ikke hente bookinger.')
    return response.json()
}

/** Retrieves upcoming sessions available for participation. */
export async function getOpenSessions(): Promise<Booking[]> {
    const response = await apiRequest(`${url}/open`, 'Kunne ikke hente økter.')
    return response.json()
}

/** Creates a booking. */
export async function createBooking(booking: BookingRequest): Promise<Booking> {
    const response = await apiRequest(url, 'Kunne ikke lagre bookingen.', {
        method: 'POST',
        body: JSON.stringify(booking),
    })
    return response.json()
}

/** Updates the selected booking. */
export async function updateBooking(id: number, booking: BookingRequest): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Kunne ikke lagre bookingen.', {
        method: 'PUT',
        body: JSON.stringify(booking),
    })
}

/** Deletes the selected booking. */
export async function deleteBooking(id: number): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Kunne ikke slette bookingen.', { method: 'DELETE' })
}

/** Joins an existing study session. */
export async function joinSession(id: number): Promise<void> {
    await apiRequest(`${url}/${id}/participants`, 'Kunne ikke bli med i økten.', { method: 'POST' })
}

/** Leaves a joined study session. */
export async function leaveSession(id: number): Promise<void> {
    await apiRequest(`${url}/${id}/participants`, 'Kunne ikke forlate økten.', { method: 'DELETE' })
}
