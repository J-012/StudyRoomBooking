/** A facility associated with one or more rooms. Returned by the API. */
export interface Amenity {
    id: number
    name: string
    description: string | null
}

/** Payload for creating or updating a amenity through the API. */
export interface AmenityRequest {
    name: string
    description: string
}
