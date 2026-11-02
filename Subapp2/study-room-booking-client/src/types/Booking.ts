export interface Booking {
    id: number
    subject: string
    topic: string
    roomId: number
    roomNumber: string
    building: string
    startTime: string
    endTime: string
    organizerName: string
    canManage: boolean
}

export interface BookingRequest {
    subject: string
    topic: string
    roomId: number
    startTime: string
    endTime: string
}