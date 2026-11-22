import type { Room, RoomRequest } from '../types/Room'
import { apiRequest } from './apiClient'

const roomsUrl = '/api/rooms'

export async function getRooms(): Promise<Room[]> {
    const response = await apiRequest(roomsUrl, 'Could not load rooms')

    return response.json()
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

    const response = await apiRequest(
        `${roomsUrl}/available?${params}`,
        'Kunne ikke hente ledige rom.'
    )

    return response.json()
}

export async function createRoom(room: RoomRequest): Promise<Room> {
    const response = await apiRequest(roomsUrl, 'Could not create room', {
        method: 'POST',
        body: JSON.stringify(room),
    })

    return response.json()
}

export async function updateRoom(
    id: number,
    room: RoomRequest
): Promise<void> {
    await apiRequest(`${roomsUrl}/${id}`, 'Could not update room', {
        method: 'PUT',
        body: JSON.stringify(room),
    })
}

export async function deleteRoom(id: number): Promise<void> {
    await apiRequest(`${roomsUrl}/${id}`, 'Could not delete room', {
        method: 'DELETE',
    })
}
