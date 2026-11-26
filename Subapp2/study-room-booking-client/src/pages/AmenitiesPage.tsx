import { getAmenities, createAmenity, updateAmenity, deleteAmenity } from '../api/amenitiesApi'
import NamedEntityPage from '../components/admin/NamedEntityPage'
import type { NamedEntity } from '../components/admin/NamedEntityPage'

/** Adapts the amenity response to the shared administration form. */
async function load(): Promise<NamedEntity[]> {
    return (await getAmenities()).map(item => ({ id: item.id, name: item.name, detail: item.description }))
}

/** Maps the shared form to the amenity create request. */
async function create(name: string, detail: string): Promise<void> {
    await createAmenity({ name, description: detail })
}

/** Maps the shared form to the amenity update request. */
async function update(id: number, name: string, detail: string): Promise<void> {
    await updateAmenity(id, { name, description: detail })
}

/** Administrator interface for amenities. */
export default function AmenitiesPage() {
    return (
        <NamedEntityPage title="Amenities" singular="Amenity"
            description="Manage facilities available in study rooms." detailLabel="Description"
            nameLimit={50} namePlaceholder="e.g. Whiteboard"
            load={load} create={create} update={update} remove={deleteAmenity} />
    )
}
