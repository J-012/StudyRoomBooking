import type { Room, RoomRequest } from '../types/Room'
import { apiRequest } from './apiClient'

const url = '/api/rooms'

/** Retrieves rooms from the API. */
export async function getRooms(): Promise<Room[]> {
    const response = await apiRequest(url, 'Could not load rooms.')
    return response.json()
}

/** Retrieves free rooms; excludes the current booking when editing. */
export async function getAvailableRooms(
    startTime: string,
    endTime: string,
    excludeBookingId?: number
): Promise<Room[]> {
    const params = new URLSearchParams({ startTime, endTime })

    if (excludeBookingId !== undefined) {
        params.set('excludeBookingId', String(excludeBookingId))
    }

    const response = await apiRequest(
        `${url}/available?${params}`,
        'Kunne ikke hente ledige rom.'
    )

    return response.json()
}

/** Creates a room. */
export async function createRoom(request: RoomRequest): Promise<Room> {
    const response = await apiRequest(url, 'Could not create room.', {
        method: 'POST',
        body: JSON.stringify(request),
    })
    return response.json()
}

/** Updates the selected room. */
export async function updateRoom(id: number, request: RoomRequest): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Could not update room.', {
        method: 'PUT',
        body: JSON.stringify(request),
    })
}

/** Deletes the selected room. */
export async function deleteRoom(id: number): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Could not delete room.', { method: 'DELETE' })
}
