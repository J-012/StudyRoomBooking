/** A campus building containing study rooms. Returned by the API. */
export interface Building {
    id: number
    name: string
    address: string | null
}

/** Payload for creating or updating a building through the API. */
export interface BuildingRequest {
    name: string
    address: string
}
