import type {
    Amenity,
    AmenityRequest,
} from '../types/Amenity'

import { clearStoredAuth } from '../context/AuthContext'

const amenitiesUrl = '/api/amenities'

function authHeaders(): HeadersInit {
    const raw = localStorage.getItem('auth')
    const token = raw
        ? JSON.parse(raw).token
        : null

    return {
        'Content-Type': 'application/json',
        ...(token
            ? { Authorization: `Bearer ${token}` }
            : {}),
    }
}

function handleUnauthorized(response: Response) {
    if (response.status === 401) {
        clearStoredAuth()
        window.location.href = '/login'
    }
}

async function getErrorMessage(
    response: Response,
    fallback: string
) {
    try {
        const data = await response.json()

        if (data.message) {
            return data.message
        }
    } catch {
        // Response had no JSON body.
    }

    return fallback
}

export async function getAmenities():
    Promise<Amenity[]> {

    const response = await fetch(
        amenitiesUrl,
        {
            headers: authHeaders(),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            'Could not load amenities'
        )
    }

    return await response.json()
}

export async function createAmenity(
    amenity: AmenityRequest
): Promise<Amenity> {

    const response = await fetch(
        amenitiesUrl,
        {
            method: 'POST',
            headers: authHeaders(),
            body: JSON.stringify(amenity),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(
                response,
                'Could not create amenity'
            )
        )
    }

    return await response.json()
}

export async function updateAmenity(
    id: number,
    amenity: AmenityRequest
): Promise<void> {

    const response = await fetch(
        `${amenitiesUrl}/${id}`,
        {
            method: 'PUT',
            headers: authHeaders(),
            body: JSON.stringify(amenity),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(
                response,
                'Could not update amenity'
            )
        )
    }
}

export async function deleteAmenity(
    id: number
): Promise<void> {

    const response = await fetch(
        `${amenitiesUrl}/${id}`,
        {
            method: 'DELETE',
            headers: authHeaders(),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            'Could not delete amenity'
        )
    }
}