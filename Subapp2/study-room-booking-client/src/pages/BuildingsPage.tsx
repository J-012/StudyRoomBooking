import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'

import {
    createBuilding,
    deleteBuilding,
    getBuildings,
    updateBuilding,
} from '../api/buildingsApi'

import type {
    Building,
    BuildingRequest,
} from '../types/Building'

const emptyForm: BuildingRequest = {
    name: '',
    address: '',
}

function BuildingsPage() {
    const [buildings, setBuildings] =
        useState<Building[]>([])

    const [form, setForm] =
        useState<BuildingRequest>(emptyForm)

    const [editingId, setEditingId] =
        useState<number | null>(null)

    const [loading, setLoading] =
        useState(true)

    const [error, setError] =
        useState('')

    async function loadBuildings() {
        try {
            setError('')

            const data =
                await getBuildings()

            setBuildings(data)
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : 'Could not load buildings.'
            )
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadBuildings()
    }, [])

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {
        event.preventDefault()

        try {
            setError('')

            if (editingId === null) {
                await createBuilding(form)
            } else {
                await updateBuilding(
                    editingId,
                    form
                )
            }

            setForm(emptyForm)
            setEditingId(null)

            await loadBuildings()
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : 'Could not save building.'
            )
        }
    }

    function startEditing(
        building: Building
    ) {
        setEditingId(building.id)

        setForm({
            name: building.name,
            address:
                building.address ?? '',
        })
    }

    function cancelEditing() {
        setEditingId(null)
        setForm(emptyForm)
    }

    async function handleDelete(
        id: number
    ) {
        const shouldDelete =
            window.confirm(
                'Delete this building?'
            )

        if (!shouldDelete) {
            return
        }

        try {
            setError('')

            await deleteBuilding(id)
            await loadBuildings()
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : 'Could not delete building.'
            )
        }
    }

    if (loading) {
        return (
            <main className="container py-5">
                <p className="text-secondary">
                    Loading buildings...
                </p>
            </main>
        )
    }

    return (
        <main className="container py-5">

            <div className="mb-4">
                <span className="section-eyebrow">
                    Facilities
                </span>

                <h1 className="h2 fw-bold mt-1 mb-2">
                    Buildings
                </h1>

                <p className="text-secondary-emphasis mb-0">
                    Manage buildings containing
                    study rooms.
                </p>
            </div>

            {error && (
                <div className="error-message mb-4">
                    {error}
                </div>
            )}

            <div className="card mb-5">
                <div className="card-body p-4">

                    <h2 className="h5 fw-bold mb-4">
                        {editingId === null
                            ? 'Add Building'
                            : 'Edit Building'}
                    </h2>

                    <form onSubmit={handleSubmit}>

                        <div className="row g-3">

                            <div className="col-12 col-md-5">
                                <label className="form-label">
                                    Name
                                </label>

                                <input
                                    className="form-control"
                                    required
                                    maxLength={100}
                                    value={form.name}
                                    placeholder="e.g. Pilestredet 35"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            name:
                                                event.target.value,
                                        })
                                    }
                                />
                            </div>

                            <div className="col-12 col-md-7">
                                <label className="form-label">
                                    Address
                                </label>

                                <input
                                    className="form-control"
                                    maxLength={200}
                                    value={form.address}
                                    placeholder="Optional address"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            address:
                                                event.target.value,
                                        })
                                    }
                                />
                            </div>

                        </div>

                        <div className="d-flex gap-2 justify-content-end mt-4">

                            {editingId !== null && (
                                <button
                                    type="button"
                                    className="btn btn-light px-4"
                                    onClick={
                                        cancelEditing
                                    }
                                >
                                    Cancel
                                </button>
                            )}

                            <button
                                type="submit"
                                className="btn btn-primary px-4"
                            >
                                {editingId === null
                                    ? 'Add Building'
                                    : 'Save Changes'}
                            </button>

                        </div>

                    </form>

                </div>
            </div>

            <div className="row g-4">

                {buildings.map(
                    (building) => (
                        <div
                            key={building.id}
                            className="col-12 col-md-6 col-lg-4"
                        >
                            <div className="card h-100">

                                <div className="card-body p-4">

                                    <h2 className="h5 fw-bold">
                                        {building.name}
                                    </h2>

                                    <p className="text-secondary mb-0">
                                        {building.address
                                            || 'No address registered.'}
                                    </p>

                                </div>

                                <div className="card-footer bg-transparent border-top-0 px-4 pb-4">

                                    <div className="d-flex gap-2">

                                        <button
                                            type="button"
                                            className="btn btn-outline-primary flex-grow-1"
                                            onClick={() =>
                                                startEditing(
                                                    building
                                                )
                                            }
                                        >
                                            Edit
                                        </button>

                                        <button
                                            type="button"
                                            className="btn btn-outline-danger"
                                            onClick={() =>
                                                handleDelete(
                                                    building.id
                                                )
                                            }
                                        >
                                            Delete
                                        </button>

                                    </div>

                                </div>

                            </div>
                        </div>
                    )
                )}

            </div>

        </main>
    )
}

export default BuildingsPage