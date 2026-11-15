import type {
    Building,
    BuildingRequest,
} from '../types/Building'

import { clearStoredAuth } from '../context/AuthContext'

const buildingsUrl = '/api/buildings'

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

export async function getBuildings():
    Promise<Building[]> {

    const response = await fetch(
        buildingsUrl,
        {
            headers: authHeaders(),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            'Could not load buildings'
        )
    }

    return await response.json()
}

export async function createBuilding(
    building: BuildingRequest
): Promise<Building> {

    const response = await fetch(
        buildingsUrl,
        {
            method: 'POST',
            headers: authHeaders(),
            body: JSON.stringify(building),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(
                response,
                'Could not create building'
            )
        )
    }

    return await response.json()
}

export async function updateBuilding(
    id: number,
    building: BuildingRequest
): Promise<void> {

    const response = await fetch(
        `${buildingsUrl}/${id}`,
        {
            method: 'PUT',
            headers: authHeaders(),
            body: JSON.stringify(building),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(
                response,
                'Could not update building'
            )
        )
    }
}

export async function deleteBuilding(
    id: number
): Promise<void> {

    const response = await fetch(
        `${buildingsUrl}/${id}`,
        {
            method: 'DELETE',
            headers: authHeaders(),
        }
    )

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error(
            await getErrorMessage(
                response,
                'Could not delete building'
            )
        )
    }
}