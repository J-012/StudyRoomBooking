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
    // Antall plasser i rommet
    capacity: number
    // Arrangøren pluss alle som har blitt med
    attendeeCount: number
    // Innlogget bruker opprettet økten
    isOrganizer: boolean
    // Innlogget bruker har blitt med i en annens økt
    isParticipant: boolean
}

export interface BookingRequest {
    subject: string
    topic: string
    roomId: number
    startTime: string
    endTime: string
}
