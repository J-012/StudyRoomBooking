import type { Building, BuildingRequest } from '../types/Building'
import { apiRequest } from './apiClient'

const url = '/api/buildings'

/** Retrieves buildings from the API. */
export async function getBuildings(): Promise<Building[]> {
    const response = await apiRequest(url, 'Could not load buildings.')
    return response.json()
}

/** Creates a building. */
export async function createBuilding(request: BuildingRequest): Promise<Building> {
    const response = await apiRequest(url, 'Could not create building.', {
        method: 'POST',
        body: JSON.stringify(request),
    })
    return response.json()
}

/** Updates the selected building. */
export async function updateBuilding(id: number, request: BuildingRequest): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Could not update building.', {
        method: 'PUT',
        body: JSON.stringify(request),
    })
}

/** Deletes the selected building. */
export async function deleteBuilding(id: number): Promise<void> {
    await apiRequest(`${url}/${id}`, 'Could not delete building.', { method: 'DELETE' })
}
