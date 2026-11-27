import type { Amenity, AmenityRequest } from '../types/Amenity'
import { apiRequest } from './apiClient'

const url = '/api/amenities'

/** Retrieves amenities from the API. */
export async function getAmenities(): Promise<Amenity[]> {
    const response = await apiRequest(url, 'Could not load amenities.')
    return response.json()
}

/** Creates a amenity. */
export async function createAmenity(request: AmenityRequest): Promise<Amenity> {
    const response = await apiRequest(url, 'Could not create amenity.', {
        method: 'POST',
        body: JSON.stringify(request),
    })
    return response.json()
}

/** Updates the selected amenity. */
export async function updateAmenity(id: number, request: AmenityRequest): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Could not update amenity.', {
        method: 'PUT',
        body: JSON.stringify(request),
    })
}

/** Deletes the selected amenity. */
export async function deleteAmenity(id: number): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Could not delete amenity.', { method: 'DELETE' })
}
