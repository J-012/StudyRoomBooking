import type { Amenity, AmenityRequest } from '../types/Amenity'
import { apiRequest } from './apiClient'

const amenitiesUrl = '/api/amenities'

export async function getAmenities(): Promise<Amenity[]> {
    const response = await apiRequest(
        amenitiesUrl,
        'Could not load amenities'
    )

    return response.json()
}

export async function createAmenity(
    amenity: AmenityRequest
): Promise<Amenity> {
    const response = await apiRequest(
        amenitiesUrl,
        'Could not create amenity',
        {
            method: 'POST',
            body: JSON.stringify(amenity),
        }
    )

    return response.json()
}

export async function updateAmenity(
    id: number,
    amenity: AmenityRequest
): Promise<void> {
    await apiRequest(
        `${amenitiesUrl}/${id}`,
        'Could not update amenity',
        {
            method: 'PUT',
            body: JSON.stringify(amenity),
        }
    )
}

export async function deleteAmenity(id: number): Promise<void> {
    await apiRequest(
        `${amenitiesUrl}/${id}`,
        'Could not delete amenity',
        {
            method: 'DELETE',
        }
    )
}
