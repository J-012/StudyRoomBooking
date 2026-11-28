import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'

import {
    createBooking,
    deleteBooking,
    getBookings,
    updateBooking,
} from '../api/bookingsApi'

import type { Booking, BookingRequest } from '../types/Booking'
import { useAsyncData } from '../hooks/useAsyncData'
import { useAvailableRooms } from '../hooks/useAvailableRooms'
import { createEmptyForm, createFormFromParams } from '../utils/bookingForm'
import { validateBookingTimes } from '../utils/bookingTime'
import type { BookingForm } from '../utils/bookingForm'
import BookingRow from '../components/bookings/BookingRow'
import BookingFormPanel from '../components/bookings/BookingFormPanel'
import { useSearchParams } from 'react-router-dom'

import { useAuth } from '../context/useAuth'

import { errorText } from '../utils/errorHandling'

import './BookingsPage.css'

/** Coordinates booking data, the form, and management actions. */
function BookingsPage() {
    const [searchParams] = useSearchParams()
    const { isAdmin } = useAuth()
    const [form, setForm] = useState<BookingForm>(() =>
        createFormFromParams(searchParams)
    )
    const [editingId, setEditingId] = useState<number | null>(null)

    const [saving, setSaving] = useState(false)
    const [showPast, setShowPast] = useState(false)
    const [now, setNow] = useState(Date.now)

    // List loading, deletion and form validation have independent error states.
    const { data, loading, error: loadError, refresh } = useAsyncData(getBookings, 'Kunne ikke hente bookinger.')
    const [actionError, setActionError] = useState('')
    const [formError, setFormError] = useState('')

    // Keep the booking list current even when the user leaves the page open.
    useEffect(() => {
        const timer = window.setInterval(() => setNow(Date.now()), 30_000)
        return () => window.clearInterval(timer)
    }, [])

    const availability = useAvailableRooms(form.date, form.start, form.end, editingId ?? undefined)
    const { rooms: availableRooms, loading: roomsLoading, valid: timesValid } = availability
    const selectedRoomId = availability.ready && availableRooms.some(room => room.id === form.roomId)
        ? form.roomId : 0

    const { upcoming, past } = useMemo(() => {
        const sorted = [...(data ?? [])].sort(
            (a, b) =>
                new Date(a.startTime).getTime() -
                new Date(b.startTime).getTime()
        )

        return {
            upcoming: sorted.filter(
                (b) => new Date(b.endTime).getTime() >= now
            ),
            past: sorted
                .filter((b) => new Date(b.endTime).getTime() < now)
                .reverse(),
        }
    }, [data, now])

    /** Updates a single typed field without mutating the form. */
    function updateField<K extends keyof BookingForm>(
        key: K,
        value: BookingForm[K]
    ) {
        setForm((current) => ({ ...current, [key]: value }))
        setFormError('')
    }

    /** Validates the selected room and creates or updates a booking. */
    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault()
        if (saving || loading) return

        const timeError = validateBookingTimes(form.date, form.start, form.end)
        if (timeError) {
            setFormError(timeError)
            return
        }

        if (roomsLoading || !availability.ready || selectedRoomId <= 0) {
            setFormError('Velg et ledig rom.')
            return
        }

        if (!form.subject.trim() || !form.topic.trim()) {
            setFormError('Fyll inn fag og tema.')
            return
        }

        const request: BookingRequest = {
            subject: form.subject.trim(),
            topic: form.topic.trim(),
            roomId: selectedRoomId,
            startTime: `${form.date}T${form.start}`,
            endTime: `${form.date}T${form.end}`,
        }

        try {
            setSaving(true)
            setFormError('')
            setActionError('')

            if (editingId === null) {
                await createBooking(request)
            } else {
                await updateBooking(editingId, request)
            }

            setForm(createEmptyForm())
            setEditingId(null)
            availability.refresh()
            refresh()
        } catch (error) {
            setFormError(errorText(error, 'Kunne ikke lagre bookingen.'))
            availability.refresh()
        } finally {
            setSaving(false)
        }
    }

    /** Prefills the form with the selected booking. */
    function startEditing(booking: Booking) {
        setEditingId(booking.id)
        setFormError('')
        setForm({
            subject: booking.subject,
            topic: booking.topic,
            roomId: booking.roomId,
            date: booking.startTime.slice(0, 10),
            start: booking.startTime.slice(11, 16),
            end: booking.endTime.slice(11, 16),
        })

        // On mobile, the form is above the list.
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    /** Resets the form and leaves edit mode. */
    function cancelEditing() {
        setEditingId(null)
        setForm(createEmptyForm())
        setFormError('')
    }

    /** Confirms deletion and refreshes bookings and room availability. */
    async function handleDelete(id: number) {
        if (saving || loading || !window.confirm('Vil du slette denne bookingen?')) {
            return
        }

        setSaving(true)
        setActionError('')
        try {
            await deleteBooking(id)
            availability.refresh()
            if (editingId === id) {
                cancelEditing()
            }
            refresh()
        } catch (error) {
            setActionError(errorText(error, 'Kunne ikke slette bookingen.'))
        } finally {
            setSaving(false)
        }
    }

    const isEditing = editingId !== null

    return (
        <main className="container py-5">
            <header className="mb-4">
                <span className="section-eyebrow">Studieøkter</span>
                <h1 className="h2 fw-bold mt-1 mb-2">Bookinger</h1>
                <p className="text-secondary mb-0">
                    Book et studierom og hold oversikt over dine økter.
                </p>
            </header>

            {actionError && (
                <div className="error-message mb-4" role="alert">{actionError}</div>
            )}

            <div className="row g-4 g-lg-5">
                {/* Booking form */}
                <div className="col-12 col-lg-4">
                    <BookingFormPanel
                        form={form}
                        isEditing={isEditing}
                        roomsLoading={roomsLoading}
                        timesValid={timesValid}
                        selectedRoomId={selectedRoomId}
                        availabilityReady={availability.ready}
                        availabilityError={availability.error}
                        refreshAvailability={availability.refresh}
                        availableRooms={availableRooms}
                        saving={saving || loading}
                        formError={formError}
                        updateField={updateField}
                        handleSubmit={handleSubmit}
                        cancelEditing={cancelEditing}
                    />
                </div>

                {/* Booking list */}
                <div className="col-12 col-lg-8">
                    <div className="d-flex justify-content-between align-items-baseline mb-3">
                        <h2 className="section-title mb-0">
                            {isAdmin
                                ? 'Alle kommende økter'
                                : 'Mine kommende økter'}
                        </h2>
                        {!loading && !loadError && (
                            <span className="text-secondary small">
                                {upcoming.length}{' '}
                                {upcoming.length === 1 ? 'booking' : 'bookinger'}
                            </span>
                        )}
                    </div>

                    {loading ? (
                        <ul className="booking-list">
                            {[0, 1, 2].map((i) => (
                                <li key={i} className="booking-row booking-skeleton loading-skeleton" aria-hidden="true" />
                            ))}
                        </ul>
                    ) : loadError ? (
                        <div className="error-message" role="alert">
                            {loadError}{' '}
                            <button type="button" className="btn btn-sm btn-light" onClick={refresh}>Prøv igjen</button>
                        </div>
                    ) : upcoming.length === 0 ? (
                        <div className="empty-state">
                            <p className="fw-semibold text-body mb-1">
                                Ingen kommende økter
                            </p>
                            <p className="mb-0">
                                Bruk skjemaet for å booke et rom.
                            </p>
                        </div>
                    ) : (
                        <ul className="booking-list">
                            {upcoming.map((booking) => (
                                <BookingRow
                                    key={booking.id}
                                    booking={booking}
                                    busy={saving}
                                    isEditing={editingId === booking.id}
                                    isPast={false}
                                    onEdit={() => startEditing(booking)}
                                    onDelete={() => handleDelete(booking.id)}
                                />
                            ))}
                        </ul>
                    )}

                    {!loading && !loadError && past.length > 0 && (
                        <div className="mt-4">
                            <button
                                type="button"
                                className="btn btn-link text-secondary text-decoration-none p-0 small"
                                onClick={() => setShowPast((v) => !v)}
                            >
                                {showPast ? 'Skjul' : 'Vis'} tidligere økter (
                                {past.length})
                            </button>

                            {showPast && (
                                <ul className="booking-list mt-3">
                                    {past.map((booking) => (
                                        <BookingRow
                                            key={booking.id}
                                            booking={booking}
                                            busy={saving}
                                            isEditing={false}
                                            isPast
                                            onEdit={() => { }}
                                            onDelete={() =>
                                                handleDelete(booking.id)
                                            }
                                        />
                                    ))}
                                </ul>
                            )}
                        </div>
                    )}
                </div>
            </div>
        </main>
    )
}

export default BookingsPage
