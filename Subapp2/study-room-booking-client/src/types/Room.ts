import type { Amenity } from './Amenity'

/** A room with building details and its associated amenities. Returned by the API. */
export interface Room {
    id: number
    roomNumber: string
    buildingId: number
    building: string
    capacity: number
    amenities: Amenity[]
}

/** Payload for creating or updating a room through the API. */
export interface RoomRequest {
    roomNumber: string
    buildingId: number
    capacity: number
    amenityIds: number[]
}
