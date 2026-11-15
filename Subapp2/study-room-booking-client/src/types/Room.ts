import type { Amenity } from './Amenity'

export interface Room {
    id: number
    roomNumber: string
    buildingId: number
    building: string
    capacity: number
    amenities: Amenity[]
}

export interface RoomRequest {
    roomNumber: string
    buildingId: number
    capacity: number
    amenityIds: number[]
}