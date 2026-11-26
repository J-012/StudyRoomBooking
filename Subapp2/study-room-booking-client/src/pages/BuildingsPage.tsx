import { getBuildings, createBuilding, updateBuilding, deleteBuilding } from '../api/buildingsApi'
import NamedEntityPage from '../components/admin/NamedEntityPage'
import type { NamedEntity } from '../components/admin/NamedEntityPage'

/** Adapts the building response to the shared administration form. */
async function load(): Promise<NamedEntity[]> {
    return (await getBuildings()).map(item => ({ id: item.id, name: item.name, detail: item.address }))
}

/** Maps the shared form to the building create request. */
async function create(name: string, detail: string): Promise<void> {
    await createBuilding({ name, address: detail })
}

/** Maps the shared form to the building update request. */
async function update(id: number, name: string, detail: string): Promise<void> {
    await updateBuilding(id, { name, address: detail })
}

/** Administrator interface for buildings. */
export default function BuildingsPage() {
    return (
        <NamedEntityPage title="Buildings" singular="Building"
            description="Manage buildings containing study rooms." detailLabel="Address"
            nameLimit={100} namePlaceholder="e.g. Pilestredet 35"
            load={load} create={create} update={update} remove={deleteBuilding} />
    )
}
