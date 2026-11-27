/** A booked study session, including permissions calculated by the server. */
export interface Booking {
    id: number
    subject: string
    topic: string

    // Room details for display; only the ID is sent when saving.
    roomId: number
    roomNumber: string
    building: string
    capacity: number
    startTime: string
    endTime: string

    organizerName: string
    // The organizer (if present) plus registered participants.
    attendeeCount: number
    isOrganizer: boolean
    isParticipant: boolean
    canManage: boolean
}

/** Create/update payload. Ownership and permissions are determined by the API. */
export interface BookingRequest {
    subject: string
    topic: string
    roomId: number
    startTime: string
    endTime: string
}
