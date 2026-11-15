import type { Amenity } from './Amenity'

export interface Room {
    id: number
    roomNumber: string
    building: string
    capacity: number
    amenities: Amenity[]
}

export interface RoomRequest {
    roomNumber: string
    building: string
    capacity: number
    amenityIds: number[]
}