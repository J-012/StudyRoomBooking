export interface Amenity {
    id: number
    name: string
    description: string | null
}

export interface AmenityRequest {
    name: string
    description: string
}