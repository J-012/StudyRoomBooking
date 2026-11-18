export interface Building {
    id: number
    name: string
    address: string | null
}

export interface BuildingRequest {
    name: string
    address: string
}