import type { Room, RoomRequest } from '../types/Room'

const roomsUrl = '/api/rooms'

export async function getRooms(): Promise<Room[]> {
    const response = await fetch(roomsUrl)

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
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(room),
    })

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
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(room),
    })

    if (!response.ok) {
        throw new Error('Could not update room')
    }
}

export async function deleteRoom(
    id: number
): Promise<void> {
    const response = await fetch(`${roomsUrl}/${id}`, {
        method: 'DELETE',
    })

    if (!response.ok) {
        throw new Error('Could not delete room')
    }
}