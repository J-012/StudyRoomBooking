import {
    useEffect,
    useState,
} from 'react'

import type {
    FormEvent,
} from 'react'

import {
    createBooking,
    deleteBooking,
    getBookings,
    updateBooking,
} from '../api/bookingsApi'

import {
    getRooms,
} from '../api/roomsApi'

import type {
    Booking,
    BookingRequest,
} from '../types/Booking'

import type {
    Room,
} from '../types/Room'

const emptyForm: BookingRequest = {
    subject: '',
    topic: '',
    roomId: 0,
    startTime: '',
    endTime: '',
}

function BookingsPage() {
    const [bookings, setBookings] =
        useState<Booking[]>([])

    const [rooms, setRooms] =
        useState<Room[]>([])

    const [form, setForm] =
        useState<BookingRequest>(
            emptyForm
        )

    const [editingId, setEditingId] =
        useState<number | null>(
            null
        )

    const [loading, setLoading] =
        useState(true)

    const [error, setError] =
        useState('')

    async function loadData() {
        try {
            setError('')

            const bookingData =
                await getBookings()

            const roomData =
                await getRooms()

            setBookings(bookingData)

            setRooms(roomData)
        } catch (error) {
            if (error instanceof Error) {
                setError(error.message)
            } else {
                setError(
                    'Could not load bookings.'
                )
            }
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadData()
    }, [])

    async function handleSubmit(
        event:
            FormEvent<HTMLFormElement>
    ) {
        event.preventDefault()

        if (form.roomId <= 0) {
            setError(
                'You must select a room.'
            )
            return
        }

        if (
            new Date(form.endTime)
            <=
            new Date(form.startTime)
        ) {
            setError(
                'End time must be after start time.'
            )
            return
        }

        try {
            setError('')

            if (editingId === null) {
                await createBooking(form)
            } else {
                await updateBooking(
                    editingId,
                    form
                )
            }

            setForm(emptyForm)

            setEditingId(null)

            await loadData()
        } catch (error) {
            if (error instanceof Error) {
                setError(error.message)
            } else {
                setError(
                    'Could not save booking.'
                )
            }
        }
    }

    function startEditing(
        booking: Booking)
    {
        setEditingId(booking.id)

        setForm({
            subject:
                booking.subject,

            topic:
                booking.topic,

            roomId:
                booking.roomId,

            startTime:
                booking.startTime
                    .slice(0, 16),

            endTime:
                booking.endTime
                    .slice(0, 16),
        })

        window.scrollTo({
            top: 0,
            behavior: 'smooth',
        })
    }

    function cancelEditing() {
        setEditingId(null)
        setForm(emptyForm)
        setError('')
    }

    async function handleDelete(
        id: number)
    {
        const shouldDelete =
            window.confirm(
                'Delete this booking?'
            )

        if (!shouldDelete) {
            return
        }

        try {
            setError('')

            await deleteBooking(id)

            await loadData()
        } catch (error) {
            if (error instanceof Error) {
                setError(error.message)
            } else {
                setError(
                    'Could not delete booking.'
                )
            }
        }
    }

    if (loading) {
        return (
            <main className="container py-5">

                <p className="text-secondary">
                    Loading bookings...
                </p>

            </main>
        )
    }

    return (
        <main className="container py-5">

            <div className="mb-4">

                <span className="section-eyebrow">
                    Study sessions
                </span>

                <h1 className="h2 fw-bold mt-1 mb-2">
                    Bookings
                </h1>

                <p className="text-secondary-emphasis mb-0">
                    Book a study room or browse
                    upcoming study sessions.
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
                            ? 'Create booking'
                            : 'Edit booking'}

                    </h2>

                    <form
                        onSubmit={
                            handleSubmit
                        }
                    >

                        <div className="row g-3">

                            <div className="col-12 col-md-6">

                                <label className="form-label">
                                    Subject
                                </label>

                                <input
                                    className="form-control"
                                    required
                                    maxLength={100}
                                    value={
                                        form.subject
                                    }
                                    onChange={
                                        (event) =>
                                            setForm({
                                                ...form,
                                                subject:
                                                    event.target.value,
                                            })
                                    }
                                />

                            </div>

                            <div className="col-12 col-md-6">

                                <label className="form-label">
                                    Topic
                                </label>

                                <input
                                    className="form-control"
                                    required
                                    maxLength={200}
                                    value={
                                        form.topic
                                    }
                                    onChange={
                                        (event) =>
                                            setForm({
                                                ...form,
                                                topic:
                                                    event.target.value,
                                            })
                                    }
                                />

                            </div>

                            <div className="col-12">

                                <label className="form-label">
                                    Room
                                </label>

                                <select
                                    className="form-select"
                                    required
                                    value={
                                        form.roomId
                                    }
                                    onChange={
                                        (event) =>
                                            setForm({
                                                ...form,
                                                roomId:
                                                    Number(
                                                        event.target.value
                                                    ),
                                            })
                                    }
                                >

                                    <option value={0}>
                                        Select room
                                    </option>

                                    {rooms.map(
                                        (room) => (
                                            <option
                                                key={
                                                    room.id
                                                }
                                                value={
                                                    room.id
                                                }
                                            >
                                                {
                                                    room.roomNumber
                                                }
                                                {' - '}
                                                {
                                                    room.building
                                                }
                                            </option>
                                        )
                                    )}

                                </select>

                            </div>

                            <div className="col-12 col-md-6">

                                <label className="form-label">
                                    Start time
                                </label>

                                <input
                                    type="datetime-local"
                                    className="form-control"
                                    required
                                    value={
                                        form.startTime
                                    }
                                    onChange={
                                        (event) =>
                                            setForm({
                                                ...form,
                                                startTime:
                                                    event.target.value,
                                            })
                                    }
                                />

                            </div>

                            <div className="col-12 col-md-6">

                                <label className="form-label">
                                    End time
                                </label>

                                <input
                                    type="datetime-local"
                                    className="form-control"
                                    required
                                    value={
                                        form.endTime
                                    }
                                    onChange={
                                        (event) =>
                                            setForm({
                                                ...form,
                                                endTime:
                                                    event.target.value,
                                            })
                                    }
                                />

                            </div>

                        </div>

                        <div className="d-flex justify-content-end gap-2 mt-4">

                            {editingId !== null && (
                                <button
                                    type="button"
                                    className="btn btn-light"
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
                                    ? 'Create booking'
                                    : 'Save changes'}
                            </button>

                        </div>

                    </form>

                </div>
            </div>

            <div className="row g-4">

                {bookings.length === 0 ? (

                    <div className="col-12">

                        <div className="card">
                            <div className="card-body text-center py-5">

                                <h2 className="h5 fw-bold">
                                    No bookings yet
                                </h2>

                                <p className="text-secondary mb-0">
                                    Create the first
                                    study session above.
                                </p>

                            </div>
                        </div>

                    </div>

                ) : (

                    bookings.map(
                        (booking) => (

                            <div
                                className="col-12 col-md-6 col-lg-4"
                                key={
                                    booking.id
                                }
                            >

                                <div className="card card-hover h-100">

                                    <div className="card-body p-4">

                                        <span className="badge badge-soft-primary rounded-pill mb-3 px-3 py-2">
                                            {
                                                booking.roomNumber
                                            }
                                        </span>

                                        <h2 className="h5 fw-bold">
                                            {
                                                booking.subject
                                            }
                                        </h2>

                                        <p className="text-secondary">
                                            {
                                                booking.topic
                                            }
                                        </p>

                                        <p className="mb-1">
                                            <strong>
                                                Room:
                                            </strong>{' '}
                                            {
                                                booking.roomNumber
                                            }
                                            {' - '}
                                            {
                                                booking.building
                                            }
                                        </p>

                                        <p className="mb-1">
                                            <strong>
                                                From:
                                            </strong>{' '}
                                            {new Date(
                                                booking.startTime
                                            ).toLocaleString()}
                                        </p>

                                        <p className="mb-1">
                                            <strong>
                                                To:
                                            </strong>{' '}
                                            {new Date(
                                                booking.endTime
                                            ).toLocaleString()}
                                        </p>

                                        <p className="text-secondary mt-3 mb-0">
                                            Organizer:{' '}
                                            {
                                                booking.organizerName
                                            }
                                        </p>

                                    </div>

                                    {booking.canManage && (

                                        <div className="card-footer bg-transparent border-top-0 px-4 pb-4">

                                            <div className="d-flex gap-2">

                                                <button
                                                    type="button"
                                                    className="btn btn-outline-primary flex-grow-1"
                                                    onClick={() =>
                                                        startEditing(
                                                            booking
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
                                                            booking.id
                                                        )
                                                    }
                                                >
                                                    Delete
                                                </button>

                                            </div>

                                        </div>

                                    )}

                                </div>

                            </div>
                        )
                    )

                )}

            </div>

        </main>
    )
}

export default BookingsPage