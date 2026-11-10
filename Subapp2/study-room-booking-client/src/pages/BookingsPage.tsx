import { useEffect, useMemo, useState } from 'react'
import type { FormEvent } from 'react'

import {
    createBooking,
    deleteBooking,
    getAvailableRooms,
    getBookings,
    updateBooking,
} from '../api/bookingsApi'

import type { Booking, BookingRequest } from '../types/Booking'
import type { Room } from '../types/Room'

import './BookingsPage.css'

// Skjemaet bruker én dato + to klokkeslett i stedet for to datetime-felt.
// Det slås sammen til startTime/endTime før det sendes til API-et.
interface BookingForm {
    subject: string
    topic: string
    roomId: number
    date: string // YYYY-MM-DD
    start: string // HH:mm
    end: string // HH:mm
}

const emptyForm: BookingForm = {
    subject: '',
    topic: '',
    roomId: 0,
    date: '',
    start: '',
    end: '',
}

function errorText(error: unknown, fallback: string) {
    return error instanceof Error ? error.message : fallback
}

function formatTime(iso: string) {
    return new Date(iso).toLocaleTimeString('nb-NO', {
        hour: '2-digit',
        minute: '2-digit',
    })
}

function formatWeekday(iso: string) {
    return new Date(iso).toLocaleDateString('nb-NO', { weekday: 'long' })
}

function todayLocal() {
    // sv-SE gir formatet YYYY-MM-DD i lokal tid
    return new Date().toLocaleDateString('sv-SE')
}

function DateBlock({ iso }: { iso: string }) {
    const date = new Date(iso)
    const month = date
        .toLocaleDateString('nb-NO', { month: 'short' })
        .replace('.', '')

    return (
        <div className="date-block">
            <span className="date-block-month">{month}</span>
            <span className="date-block-day">{date.getDate()}</span>
        </div>
    )
}

interface BookingRowProps {
    booking: Booking
    isEditing: boolean
    isPast: boolean
    onEdit: () => void
    onDelete: () => void
}

function BookingRow({
    booking,
    isEditing,
    isPast,
    onEdit,
    onDelete,
}: BookingRowProps) {
    const classes = [
        'booking-row',
        isEditing ? 'is-editing' : '',
        isPast ? 'is-past' : '',
    ].join(' ')

    return (
        <li className={classes}>
            <DateBlock iso={booking.startTime} />

            <div className="booking-row-body">
                <div className="d-flex flex-wrap align-items-center gap-2 mb-1">
                    <h3 className="booking-row-title">{booking.subject}</h3>
                    <span className="booking-chip">
                        {booking.roomNumber} · {booking.building}
                    </span>
                </div>

                <p className="booking-row-topic">{booking.topic}</p>

                <p className="booking-row-meta">
                    <span className="text-capitalize">
                        {formatWeekday(booking.startTime)}
                    </span>
                    , {formatTime(booking.startTime)}–
                    {formatTime(booking.endTime)}
                    <span className="booking-row-dot">·</span>
                    {booking.organizerName}
                </p>
            </div>

            {booking.canManage && (
                <div className="booking-row-actions">
                    {!isPast && (
                        <button
                            type="button"
                            className="btn btn-sm btn-light"
                            onClick={onEdit}
                        >
                            Endre
                        </button>
                    )}
                    <button
                        type="button"
                        className="btn btn-sm btn-link text-danger text-decoration-none"
                        onClick={onDelete}
                    >
                        Slett
                    </button>
                </div>
            )}
        </li>
    )
}

function BookingsPage() {
    const [bookings, setBookings] = useState<Booking[]>([])
    const [availableRooms, setAvailableRooms] = useState<Room[]>([])
    const [roomsLoading, setRoomsLoading] = useState(false)
    const [form, setForm] = useState<BookingForm>(emptyForm)
    const [editingId, setEditingId] = useState<number | null>(null)

    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)
    const [showPast, setShowPast] = useState(false)

    // Skiller mellom feil ved lasting (vises øverst)
    // og feil i skjemaet (vises inne i skjemaet).
    const [loadError, setLoadError] = useState('')
    const [formError, setFormError] = useState('')

    async function loadData() {
        try {
            setLoadError('')
            setBookings(await getBookings())
        } catch (error) {
            setLoadError(errorText(error, 'Kunne ikke hente bookinger.'))
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadData()
    }, [])

    const timesValid =
        form.date !== '' &&
        form.start !== '' &&
        form.end !== '' &&
        form.end > form.start

    // Hent ledige rom hver gang dato/tid endres (med litt forsinkelse,
    // så vi ikke spør API-et for hvert tastetrykk).
    useEffect(() => {
        if (!timesValid) {
            setAvailableRooms([])
            setRoomsLoading(false)
            return
        }

        let cancelled = false
        setRoomsLoading(true)

        const timer = setTimeout(async () => {
            try {
                const rooms = await getAvailableRooms(
                    `${form.date}T${form.start}`,
                    `${form.date}T${form.end}`,
                    editingId ?? undefined
                )
                if (cancelled) return

                setAvailableRooms(rooms)
                // Nullstill rommet hvis det ikke lenger er ledig
                setForm((current) =>
                    rooms.some((r) => r.id === current.roomId)
                        ? current
                        : { ...current, roomId: 0 }
                )
            } catch (error) {
                if (!cancelled) {
                    setFormError(
                        errorText(error, 'Kunne ikke hente ledige rom.')
                    )
                }
            } finally {
                if (!cancelled) setRoomsLoading(false)
            }
        }, 300)

        return () => {
            cancelled = true
            clearTimeout(timer)
        }
    }, [form.date, form.start, form.end, editingId, timesValid])

    const { upcoming, past } = useMemo(() => {
        const now = Date.now()
        const sorted = [...bookings].sort(
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
    }, [bookings])

    function updateField<K extends keyof BookingForm>(
        key: K,
        value: BookingForm[K]
    ) {
        setForm((current) => ({ ...current, [key]: value }))
    }

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault()

        if (form.end <= form.start) {
            setFormError('Sluttid må være etter starttid.')
            return
        }

        if (form.roomId <= 0) {
            setFormError('Velg et ledig rom.')
            return
        }

        const request: BookingRequest = {
            subject: form.subject,
            topic: form.topic,
            roomId: form.roomId,
            startTime: `${form.date}T${form.start}`,
            endTime: `${form.date}T${form.end}`,
        }

        try {
            setSaving(true)
            setFormError('')

            if (editingId === null) {
                await createBooking(request)
            } else {
                await updateBooking(editingId, request)
            }

            setForm(emptyForm)
            setEditingId(null)
            await loadData()
        } catch (error) {
            setFormError(errorText(error, 'Kunne ikke lagre bookingen.'))
        } finally {
            setSaving(false)
        }
    }

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

        // På mobil ligger skjemaet over listen
        window.scrollTo({ top: 0, behavior: 'smooth' })
    }

    function cancelEditing() {
        setEditingId(null)
        setForm(emptyForm)
        setFormError('')
    }

    async function handleDelete(id: number) {
        if (!window.confirm('Vil du slette denne bookingen?')) {
            return
        }

        try {
            await deleteBooking(id)
            if (editingId === id) {
                cancelEditing()
            }
            await loadData()
        } catch (error) {
            setLoadError(errorText(error, 'Kunne ikke slette bookingen.'))
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

            {loadError && (
                <div className="error-message mb-4">{loadError}</div>
            )}

            <div className="row g-4 g-lg-5">
                {/* Skjema: venstre kolonne på desktop, øverst på mobil */}
                <div className="col-12 col-lg-4">
                    <aside
                        className={`card booking-panel ${isEditing ? 'is-editing' : ''}`}
                    >
                        <div className="card-body p-4">
                            <div className="d-flex justify-content-between align-items-center mb-3">
                                <h2 className="h5 fw-bold mb-0">
                                    {isEditing ? 'Endre booking' : 'Ny booking'}
                                </h2>
                                {isEditing && (
                                    <button
                                        type="button"
                                        className="btn btn-sm btn-link text-secondary text-decoration-none p-0"
                                        onClick={cancelEditing}
                                    >
                                        Avbryt
                                    </button>
                                )}
                            </div>

                            <form
                                onSubmit={handleSubmit}
                                className="d-flex flex-column gap-3"
                            >
                                <div>
                                    <label className="form-label" htmlFor="date">
                                        Dato
                                    </label>
                                    <input
                                        id="date"
                                        type="date"
                                        className="form-control"
                                        required
                                        min={isEditing ? undefined : todayLocal()}
                                        value={form.date}
                                        onChange={(e) =>
                                            updateField('date', e.target.value)
                                        }
                                    />
                                </div>

                                <div className="row g-2">
                                    <div className="col-6">
                                        <label className="form-label" htmlFor="start">
                                            Fra
                                        </label>
                                        <input
                                            id="start"
                                            type="time"
                                            className="form-control"
                                            required
                                            step={900}
                                            value={form.start}
                                            onChange={(e) =>
                                                updateField('start', e.target.value)
                                            }
                                        />
                                    </div>
                                    <div className="col-6">
                                        <label className="form-label" htmlFor="end">
                                            Til
                                        </label>
                                        <input
                                            id="end"
                                            type="time"
                                            className="form-control"
                                            required
                                            step={900}
                                            value={form.end}
                                            onChange={(e) =>
                                                updateField('end', e.target.value)
                                            }
                                        />
                                    </div>
                                </div>

                                {form.start !== '' &&
                                    form.end !== '' &&
                                    form.end <= form.start && (
                                        <div className="form-hint text-danger">
                                            Sluttid må være etter starttid.
                                        </div>
                                    )}

                                <div>
                                    <span className="form-label d-block">
                                        Ledige rom
                                        {timesValid && !roomsLoading && (
                                            <span className="text-secondary fw-normal">
                                                {' '}({availableRooms.length})
                                            </span>
                                        )}
                                    </span>

                                    {!timesValid ? (
                                        <div className="room-placeholder">
                                            Velg dato og tid for å se ledige rom.
                                        </div>
                                    ) : roomsLoading ? (
                                        <div className="room-placeholder">
                                            Henter ledige rom…
                                        </div>
                                    ) : availableRooms.length === 0 ? (
                                        <div className="room-placeholder">
                                            Ingen ledige rom i dette tidsrommet.
                                            Prøv et annet tidspunkt.
                                        </div>
                                    ) : (
                                        <div
                                            className="room-options"
                                            role="radiogroup"
                                            aria-label="Ledige rom"
                                        >
                                            {availableRooms.map((room) => {
                                                const selected =
                                                    form.roomId === room.id
                                                return (
                                                    <button
                                                        key={room.id}
                                                        type="button"
                                                        role="radio"
                                                        aria-checked={selected}
                                                        className={`room-option ${selected ? 'is-selected' : ''}`}
                                                        onClick={() =>
                                                            updateField(
                                                                'roomId',
                                                                room.id
                                                            )
                                                        }
                                                    >
                                                        <span className="room-option-number">
                                                            {room.roomNumber}
                                                        </span>
                                                        <span className="room-option-building">
                                                            {room.building}
                                                        </span>
                                                    </button>
                                                )
                                            })}
                                        </div>
                                    )}
                                </div>

                                <hr className="my-1" />

                                <div>
                                    <label className="form-label" htmlFor="subject">
                                        Fag
                                    </label>
                                    <input
                                        id="subject"
                                        className="form-control"
                                        placeholder="F.eks. DATA2410"
                                        required
                                        maxLength={100}
                                        value={form.subject}
                                        onChange={(e) =>
                                            updateField('subject', e.target.value)
                                        }
                                    />
                                </div>

                                <div>
                                    <label className="form-label" htmlFor="topic">
                                        Tema
                                    </label>
                                    <input
                                        id="topic"
                                        className="form-control"
                                        placeholder="Hva skal dere jobbe med?"
                                        required
                                        maxLength={200}
                                        value={form.topic}
                                        onChange={(e) =>
                                            updateField('topic', e.target.value)
                                        }
                                    />
                                </div>

                                {formError && (
                                    <div className="form-error">{formError}</div>
                                )}

                                <button
                                    type="submit"
                                    className="btn btn-primary w-100 py-2"
                                    disabled={saving || form.roomId <= 0}
                                >
                                    {saving
                                        ? 'Lagrer…'
                                        : isEditing
                                          ? 'Lagre endringer'
                                          : 'Book rom'}
                                </button>
                            </form>
                        </div>
                    </aside>
                </div>

                {/* Liste: høyre kolonne på desktop */}
                <div className="col-12 col-lg-8">
                    <div className="d-flex justify-content-between align-items-baseline mb-3">
                        <h2 className="section-title mb-0">Mine kommende økter</h2>
                        {!loading && (
                            <span className="text-secondary small">
                                {upcoming.length}{' '}
                                {upcoming.length === 1 ? 'booking' : 'bookinger'}
                            </span>
                        )}
                    </div>

                    {loading ? (
                        <ul className="booking-list">
                            {[0, 1, 2].map((i) => (
                                <li key={i} className="booking-row skeleton" />
                            ))}
                        </ul>
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
                                    isEditing={editingId === booking.id}
                                    isPast={false}
                                    onEdit={() => startEditing(booking)}
                                    onDelete={() => handleDelete(booking.id)}
                                />
                            ))}
                        </ul>
                    )}

                    {past.length > 0 && (
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
                                            isEditing={false}
                                            isPast
                                            onEdit={() => {}}
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
