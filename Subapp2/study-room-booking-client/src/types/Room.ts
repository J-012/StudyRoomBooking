export interface Room {
    id: number
    roomNumber: string
    building: string
    capacity: number
}

export interface RoomRequest {
    roomNumber: string
    building: string
    capacity: number
}