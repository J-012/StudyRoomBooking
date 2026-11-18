import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'

import {
    createAmenity,
    deleteAmenity,
    getAmenities,
    updateAmenity,
} from '../api/amenitiesApi'

import type {
    Amenity,
    AmenityRequest,
} from '../types/Amenity'

const emptyForm: AmenityRequest = {
    name: '',
    description: '',
}

function AmenitiesPage() {
    const [amenities, setAmenities] =
        useState<Amenity[]>([])

    const [form, setForm] =
        useState<AmenityRequest>(emptyForm)

    const [editingId, setEditingId] =
        useState<number | null>(null)

    const [loading, setLoading] =
        useState(true)

    const [error, setError] =
        useState('')

    async function loadAmenities() {
        try {
            setError('')

            const data =
                await getAmenities()

            setAmenities(data)
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : 'Could not load amenities.'
            )
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadAmenities()
    }, [])

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {
        event.preventDefault()

        try {
            setError('')

            if (editingId === null) {
                await createAmenity(form)
            } else {
                await updateAmenity(
                    editingId,
                    form
                )
            }

            setForm(emptyForm)
            setEditingId(null)

            await loadAmenities()
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : 'Could not save amenity.'
            )
        }
    }

    function startEditing(
        amenity: Amenity
    ) {
        setEditingId(amenity.id)

        setForm({
            name: amenity.name,
            description:
                amenity.description ?? '',
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
                'Delete this amenity?'
            )

        if (!shouldDelete) {
            return
        }

        try {
            setError('')

            await deleteAmenity(id)
            await loadAmenities()
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : 'Could not delete amenity.'
            )
        }
    }

    if (loading) {
        return (
            <main className="container py-5">
                <p className="text-secondary">
                    Loading amenities...
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
                    Amenities
                </h1>

                <p className="text-secondary-emphasis mb-0">
                    Manage facilities available
                    in study rooms.
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
                            ? 'Add Amenity'
                            : 'Edit Amenity'}
                    </h2>

                    <form onSubmit={handleSubmit}>

                        <div className="row g-3">

                            <div className="col-12 col-md-4">
                                <label className="form-label">
                                    Name
                                </label>

                                <input
                                    className="form-control"
                                    required
                                    maxLength={50}
                                    value={form.name}
                                    placeholder="e.g. Whiteboard"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            name:
                                                event.target.value,
                                        })
                                    }
                                />
                            </div>

                            <div className="col-12 col-md-8">
                                <label className="form-label">
                                    Description
                                </label>

                                <input
                                    className="form-control"
                                    maxLength={200}
                                    value={form.description}
                                    placeholder="Optional description"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            description:
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
                                    ? 'Add Amenity'
                                    : 'Save Changes'}
                            </button>

                        </div>

                    </form>

                </div>
            </div>

            <div className="row g-4">

                {amenities.map(
                    (amenity) => (
                        <div
                            key={amenity.id}
                            className="col-12 col-md-6 col-lg-4"
                        >
                            <div className="card h-100">

                                <div className="card-body p-4">

                                    <h2 className="h5 fw-bold">
                                        {amenity.name}
                                    </h2>

                                    <p className="text-secondary mb-0">
                                        {amenity.description
                                            || 'No description.'}
                                    </p>

                                </div>

                                <div className="card-footer bg-transparent border-top-0 px-4 pb-4">

                                    <div className="d-flex gap-2">

                                        <button
                                            type="button"
                                            className="btn btn-outline-primary flex-grow-1"
                                            onClick={() =>
                                                startEditing(
                                                    amenity
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
                                                    amenity.id
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

export default AmenitiesPage