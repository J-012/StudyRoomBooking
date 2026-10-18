import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'

import {
    createRoom,
    deleteRoom,
    getRooms,
    updateRoom,
} from '../api/roomsApi'

import type {
    Room,
    RoomRequest,
} from '../types/Room'

const emptyForm: RoomRequest = {
    roomNumber: '',
    building: '',
    capacity: 1,
}

function RoomsPage() {
    const [rooms, setRooms] = useState<Room[]>([])
    const [form, setForm] = useState<RoomRequest>(emptyForm)

    const [editingId, setEditingId] =
        useState<number | null>(null)

    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    async function loadRooms() {
        try {
            setError('')

            const roomData = await getRooms()

            setRooms(roomData)
        } catch {
            setError('Could not load rooms.')
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadRooms()
    }, [])

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {
        event.preventDefault()

        try {
            setError('')

            if (editingId === null) {
                await createRoom(form)
            } else {
                await updateRoom(editingId, form)
            }

            setForm(emptyForm)
            setEditingId(null)

            await loadRooms()
        } catch {
            setError('Could not save room.')
        }
    }

    function startEditing(room: Room) {
        setEditingId(room.id)

        setForm({
            roomNumber: room.roomNumber,
            building: room.building,
            capacity: room.capacity,
        })
    }

    function cancelEditing() {
        setEditingId(null)
        setForm(emptyForm)
    }

    async function handleDelete(id: number) {
        const shouldDelete =
            window.confirm('Delete this room?')

        if (!shouldDelete) {
            return
        }

        try {
            await deleteRoom(id)
            await loadRooms()
        } catch {
            setError('Could not delete room.')
        }
    }

    if (loading) {
        return (
            <main className="container py-5">
                <p className="text-secondary">
                    Loading rooms...
                </p>
            </main>
        )
    }
    
    return (
        <main className="container py-5">
    
            <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-end gap-3 mb-4">
    
                <div>
                    <span className="section-eyebrow">
                        Facilities
                    </span>
    
                    <h1 className="h2 fw-bold mt-1 mb-2">
                        Study Rooms
                    </h1>
    
                    <p className="text-secondary-emphasis mb-0">
                        {rooms.length > 0
                            ? `${rooms.length} ${
                                  rooms.length === 1
                                      ? 'room'
                                      : 'rooms'
                              } available for booking.`
                            : 'Browse the rooms that are available to book.'}
                    </p>
                </div>
    
            </div>
    
            {error && (
                <div className="error-message mb-4">
                    {error}
                </div>
            )}
    
            <div className="card mb-5">
                <div className="card-body p-4">
    
                    <span className="section-eyebrow">
                        Room management
                    </span>
    
                    <h2 className="h5 fw-bold mt-1 mb-4">
                        {editingId === null
                            ? 'Add Room'
                            : 'Edit Room'}
                    </h2>
    
                    <form onSubmit={handleSubmit}>
    
                        <div className="row g-3">
    
                            <div className="col-12 col-md-4">
    
                                <label className="form-label">
                                    Room number
                                </label>
    
                                <input
                                    className="form-control"
                                    value={form.roomNumber}
                                    required
                                    maxLength={20}
                                    placeholder="e.g. PH170"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            roomNumber:
                                                event.target.value,
                                        })
                                    }
                                />
    
                            </div>
    
                            <div className="col-12 col-md-4">
    
                                <label className="form-label">
                                    Building
                                </label>
    
                                <input
                                    className="form-control"
                                    value={form.building}
                                    required
                                    maxLength={100}
                                    placeholder="e.g. Pilestredet 35"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            building:
                                                event.target.value,
                                        })
                                    }
                                />
    
                            </div>
    
                            <div className="col-12 col-md-4">
    
                                <label className="form-label">
                                    Capacity
                                </label>
    
                                <input
                                    className="form-control"
                                    type="number"
                                    min="1"
                                    max="100"
                                    value={form.capacity}
                                    required
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            capacity: Number(
                                                event.target.value
                                            ),
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
                                    onClick={cancelEditing}
                                >
                                    Cancel
                                </button>
                            )}
    
                            <button
                                type="submit"
                                className="btn btn-primary px-4"
                            >
                                {editingId === null
                                    ? 'Add Room'
                                    : 'Save Changes'}
                            </button>
    
                        </div>
    
                    </form>
    
                </div>
            </div>
    
            {rooms.length > 0 ? (
    
                <div className="row g-4">
    
                    {rooms.map((room) => (
    
                        <div
                            className="col-12 col-sm-6 col-lg-4"
                            key={room.id}
                        >
    
                            <div className="card card-hover h-100">
    
                                <div className="card-body p-4 d-flex flex-column">
    
                                    <div className="d-flex justify-content-end mb-3">
    
                                        <span className="badge badge-soft-primary rounded-pill px-3 py-2">
    
                                            {room.capacity}{' '}
                                            {room.capacity === 1
                                                ? 'seat'
                                                : 'seats'}
    
                                        </span>
    
                                    </div>
    
                                    <h2 className="h5 fw-bold mb-1">
                                        Room {room.roomNumber}
                                    </h2>
    
                                    <p className="text-secondary-emphasis mb-0">
                                        {room.building}
                                    </p>
    
                                </div>
    
                                <div className="card-footer bg-transparent border-top-0 px-4 pb-4">
    
                                    <div className="d-flex gap-2">
    
                                        <button
                                            type="button"
                                            className="btn btn-outline-primary flex-grow-1"
                                            onClick={() =>
                                                startEditing(room)
                                            }
                                        >
                                            Edit
                                        </button>
    
                                        <button
                                            type="button"
                                            className="btn btn-outline-danger"
                                            onClick={() =>
                                                handleDelete(room.id)
                                            }
                                        >
                                            Delete
                                        </button>
    
                                    </div>
    
                                </div>
    
                            </div>
    
                        </div>
    
                    ))}
    
                </div>
    
            ) : (
    
                <div className="card">
    
                    <div className="card-body text-center py-5">
    
                        <h2 className="h5 fw-bold">
                            No study rooms available
                        </h2>
    
                        <p className="text-secondary-emphasis mb-0">
                            There are no rooms registered yet.
                        </p>
    
                    </div>
    
                </div>
    
            )}
    
        </main>
    )
}

export default RoomsPage