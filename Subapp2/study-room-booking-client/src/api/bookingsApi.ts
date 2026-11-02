import type {
    Booking,
    BookingRequest,
} from '../types/Booking'

import { clearStoredAuth } from '../context/AuthContext'

const bookingsUrl = '/api/bookings'

function authHeaders(): HeadersInit {
    const raw = localStorage.getItem('auth')

    const token =
        raw
            ? JSON.parse(raw).token
            : null

    return {
        'Content-Type': 'application/json',

        ...(token
            ? {
                  Authorization:
                      `Bearer ${token}`,
              }
            : {}),
    }
}

function handleUnauthorized(
    response: Response)
{
    if (response.status === 401) {
        clearStoredAuth()
        window.location.href = '/login'
    }
}

async function getErrorMessage(
    response: Response)
{
    try {
        const data = await response.json()

        if (data.message) {
            return data.message
        }
    } catch {
        // Response did not contain JSON.
    }

    if (response.status === 403) {
        return 'You are not allowed to change this booking.'
    }

    return 'Something went wrong.'
}

export async function getBookings():
    Promise<Booking[]>
{
    const response =
        await fetch(
            bookingsUrl,
            {
                headers: authHeaders(),
            }
        )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(response)
        )
    }

    return response.json()
}

export async function createBooking(
    booking: BookingRequest
): Promise<Booking>
{
    const response =
        await fetch(
            bookingsUrl,
            {
                method: 'POST',
                headers: authHeaders(),
                body: JSON.stringify(booking),
            }
        )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(response)
        )
    }

    return response.json()
}

export async function updateBooking(
    id: number,
    booking: BookingRequest
): Promise<void>
{
    const response =
        await fetch(
            `${bookingsUrl}/${id}`,
            {
                method: 'PUT',
                headers: authHeaders(),
                body: JSON.stringify(booking),
            }
        )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(response)
        )
    }
}

export async function deleteBooking(
    id: number
): Promise<void>
{
    const response =
        await fetch(
            `${bookingsUrl}/${id}`,
            {
                method: 'DELETE',
                headers: authHeaders(),
            }
        )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(response)
        )
    }
}