import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { createRoom, deleteRoom, getRooms, updateRoom } from '../api/roomsApi'
import { getAmenities } from '../api/amenitiesApi'
import { getBuildings } from '../api/buildingsApi'
import type { Amenity } from '../types/Amenity'
import type { Building } from '../types/Building'
import type { Room, RoomRequest } from '../types/Room'
import { errorText } from '../utils/errorHandling'
import AmenityBadges from '../components/AmenityBadges'

const emptyForm: RoomRequest = {
    roomNumber: '',
    buildingId: 0,
    capacity: 1,
    amenityIds: [],
}

/** Admin page for room management, including building and amenity relationships. */
export default function RoomsPage() {
    const [rooms, setRooms] = useState<Room[]>([])
    const [amenities, setAmenities] = useState<Amenity[]>([])
    const [buildings, setBuildings] = useState<Building[]>([])
    const [form, setForm] = useState<RoomRequest>(emptyForm)
    const [editingId, setEditingId] = useState<number | null>(null)
    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)
    const [error, setError] = useState('')
    const [loaded, setLoaded] = useState(false)

    // Load all three collections together so the form cannot submit incomplete relationships.
    useEffect(() => {
        let cancelled = false
        Promise.all([getRooms(), getBuildings(), getAmenities()])
            .then(([roomData, buildingData, amenityData]) => {
                if (cancelled) return
                setRooms(roomData)
                setBuildings(buildingData)
                setAmenities(amenityData)
                setLoaded(true)
            })
            .catch(error => {
                if (!cancelled) setError(errorText(error, 'Could not load room data.'))
            })
            .finally(() => {
                if (!cancelled) setLoading(false)
            })
        return () => { cancelled = true }
    }, [])

    /** Resets the form to create mode. */
    function cancelEditing() {
        setEditingId(null)
        setForm(emptyForm)
    }

    /** Copies the selected room and its relationship IDs into the form. */
    function startEditing(room: Room) {
        setEditingId(room.id)
        setForm({
            roomNumber: room.roomNumber,
            buildingId: room.buildingId,
            capacity: room.capacity,
            amenityIds: room.amenities.map(amenity => amenity.id),
        })
        setError('')
    }

    /** Adds or removes an amenity ID without mutating the current state. */
    function toggleAmenity(amenityId: number) {
        setForm(current => ({
            ...current,
            amenityIds: current.amenityIds.includes(amenityId)
                ? current.amenityIds.filter(id => id !== amenityId)
                : [...current.amenityIds, amenityId],
        }))
    }

    /** Validates input before saving; the API independently validates all relationships. */
    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault()
        if (saving || !loaded) return
        if (!form.roomNumber.trim()) {
            setError('Room number is required.')
            return
        }
        if (!buildings.some(building => building.id === form.buildingId)) {
            setError('Please select a building.')
            return
        }
        if (!Number.isInteger(form.capacity) || form.capacity < 1 || form.capacity > 100) {
            setError('Capacity must be between 1 and 100.')
            return
        }

        setSaving(true)
        setError('')
        try {
            const request = { ...form, roomNumber: form.roomNumber.trim() }
            if (editingId === null) await createRoom(request)
            else await updateRoom(editingId, request)
            cancelEditing()
            setRooms(await getRooms())
        } catch (error) {
            setError(errorText(error, 'Could not save room.'))
        } finally {
            setSaving(false)
        }
    }

    /** Deletes a confirmed room and updates the list after the API succeeds. */
    async function handleDelete(id: number) {
        if (saving || !window.confirm('Delete this room?')) return
        setSaving(true)
        setError('')
        try {
            await deleteRoom(id)
            if (editingId === id) cancelEditing()
            setRooms(await getRooms())
        } catch (error) {
            setError(errorText(error, 'Could not delete room.'))
        } finally {
            setSaving(false)
        }
    }

    return (
        <main className="container py-5">
            <header className="mb-4">
                <span className="section-eyebrow">Facilities</span>
                <h1 className="h2 fw-bold mt-1 mb-2">Study Rooms</h1>
                <p className="text-secondary-emphasis mb-0">
                    {rooms.length > 0 ? `${rooms.length} ${rooms.length === 1 ? 'room' : 'rooms'} available for booking.` : 'Browse the rooms that are available to book.'}
                </p>
            </header>
            {error && <div className="error-message mb-4" role="alert">{error}</div>}
            {loading ? <p className="text-secondary">Loading rooms...</p> : loaded && (
                <>
                    <div className="card mb-5">
                        <div className="card-body p-4">
                            <h2 className="h5 fw-bold mb-4">{editingId === null ? 'Add Room' : 'Edit Room'}</h2>
                            <form onSubmit={handleSubmit}>
                                <fieldset disabled={saving}>
                                    <div className="row g-3">
                                        <div className="col-12 col-md-4">
                                            <label className="form-label" htmlFor="room-number">Room number</label>
                                            <input id="room-number" className="form-control" required maxLength={20}
                                                placeholder="e.g. PH170" value={form.roomNumber}
                                                onChange={event => setForm({ ...form, roomNumber: event.target.value })} />
                                        </div>
                                        <div className="col-12 col-md-4">
                                            <label className="form-label" htmlFor="building-id">Building</label>
                                            <select id="building-id" className="form-select" required value={form.buildingId || ''}
                                                onChange={event => setForm({ ...form, buildingId: Number(event.target.value) })}>
                                                <option value="">Select building</option>
                                                {buildings.map(building => <option key={building.id} value={building.id}>{building.name}</option>)}
                                            </select>
                                        </div>
                                        <div className="col-12 col-md-4">
                                            <label className="form-label" htmlFor="capacity">Capacity</label>
                                            <input id="capacity" className="form-control" type="number" min={1} max={100} required
                                                value={form.capacity} onChange={event => setForm({ ...form, capacity: Number(event.target.value) })} />
                                        </div>
                                    </div>
                                    {amenities.length > 0 && (
                                        <fieldset className="mt-4">
                                            <legend className="form-label fw-semibold fs-6">Amenities</legend>
                                            <div className="d-flex flex-wrap gap-3">
                                                {amenities.map(amenity => (
                                                    <div className="form-check" key={amenity.id}>
                                                        <input id={`amenity-${amenity.id}`} className="form-check-input" type="checkbox"
                                                            checked={form.amenityIds.includes(amenity.id)} onChange={() => toggleAmenity(amenity.id)} />
                                                        <label className="form-check-label" htmlFor={`amenity-${amenity.id}`}>{amenity.name}</label>
                                                    </div>
                                                ))}
                                            </div>
                                        </fieldset>
                                    )}
                                    <div className="d-flex gap-2 justify-content-end mt-4">
                                        {editingId !== null && <button type="button" className="btn btn-light px-4" onClick={cancelEditing}>Cancel</button>}
                                        <button type="submit" className="btn btn-primary px-4" disabled={saving || buildings.length === 0}>
                                            {saving ? 'Saving…' : editingId === null ? 'Add Room' : 'Save Changes'}
                                        </button>
                                    </div>
                                </fieldset>
                            </form>
                        </div>
                    </div>
                    {rooms.length === 0 ? (
                        <div className="card"><div className="card-body text-center py-5">
                            <h2 className="h5 fw-bold">No study rooms available</h2>
                            <p className="text-secondary-emphasis mb-0">There are no rooms registered yet.</p>
                        </div></div>
                    ) : (
                        <div className="row g-4">
                            {rooms.map(room => (
                                <div className="col-12 col-sm-6 col-lg-4" key={room.id}>
                                    <div className="card card-hover h-100">
                                        <div className="card-body p-4 d-flex flex-column">
                                            <div className="d-flex justify-content-end mb-3">
                                                <span className="badge badge-soft-primary rounded-pill px-3 py-2">{room.capacity} {room.capacity === 1 ? 'seat' : 'seats'}</span>
                                            </div>
                                            <h2 className="h5 fw-bold mb-1">Room {room.roomNumber}</h2>
                                            <p className="text-secondary-emphasis mb-0">{room.building}</p>
                                            <AmenityBadges amenities={room.amenities} className="mt-3" />
                                        </div>
                                        <div className="card-footer bg-transparent border-top-0 px-4 pb-4">
                                            <div className="d-flex gap-2">
                                                <button type="button" className="btn btn-outline-primary flex-grow-1" disabled={saving} onClick={() => startEditing(room)}>Edit</button>
                                                <button type="button" className="btn btn-outline-danger" disabled={saving} onClick={() => handleDelete(room.id)}>Delete</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </>
            )}
        </main>
    )
}
