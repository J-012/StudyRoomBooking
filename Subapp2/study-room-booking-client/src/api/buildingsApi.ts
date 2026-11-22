import type { Building, BuildingRequest } from '../types/Building'
import { apiRequest } from './apiClient'

const buildingsUrl = '/api/buildings'

export async function getBuildings(): Promise<Building[]> {
    const response = await apiRequest(
        buildingsUrl,
        'Could not load buildings'
    )

    return response.json()
}

export async function createBuilding(
    building: BuildingRequest
): Promise<Building> {
    const response = await apiRequest(
        buildingsUrl,
        'Could not create building',
        {
            method: 'POST',
            body: JSON.stringify(building),
        }
    )

    return response.json()
}

export async function updateBuilding(
    id: number,
    building: BuildingRequest
): Promise<void> {
    await apiRequest(
        `${buildingsUrl}/${id}`,
        'Could not update building',
        {
            method: 'PUT',
            body: JSON.stringify(building),
        }
    )
}

export async function deleteBuilding(id: number): Promise<void> {
    await apiRequest(
        `${buildingsUrl}/${id}`,
        'Could not delete building',
        {
            method: 'DELETE',
        }
    )
}
