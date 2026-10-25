import type { Room, RoomRequest } from '../types/Room'
import { clearStoredAuth } from '../context/AuthContext'

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

export async function getRooms(): Promise<Room[]> {
    const response = await fetch(roomsUrl, {
        headers: authHeaders(),
    })

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error('Could not load rooms')
    }

    const rooms: Room[] = await response.json()

    return rooms
}

export async function createRoom(
    room: RoomRequest
): Promise<Room> {
    const response = await fetch(roomsUrl, {
        method: 'POST',
        headers: authHeaders(),
        body: JSON.stringify(room),
    })

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error('Could not create room')
    }

    const createdRoom: Room = await response.json()

    return createdRoom
}

export async function updateRoom(
    id: number,
    room: RoomRequest
): Promise<void> {
    const response = await fetch(`${roomsUrl}/${id}`, {
        method: 'PUT',
        headers: authHeaders(),
        body: JSON.stringify(room),
    })

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error('Could not update room')
    }
}

export async function deleteRoom(
    id: number
): Promise<void> {
    const response = await fetch(`${roomsUrl}/${id}`, {
        method: 'DELETE',
        headers: authHeaders(),
    })

    handleUnauthorized(response)

    if (!response.ok) {
        throw new Error('Could not delete room')
    }
}