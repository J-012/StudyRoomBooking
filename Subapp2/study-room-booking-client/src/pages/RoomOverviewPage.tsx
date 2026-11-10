import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'

import { getAvailableRooms } from '../api/bookingsApi'
import { getRooms } from '../api/roomsApi'
import { useAuth } from '../context/AuthContext'
import type { Room } from '../types/Room'

import './RoomOverviewPage.css'

type RoomStatus = 'free' | 'busy' | 'checking' | null

const capacityOptions = [0, 2, 4, 6, 8, 10]

function pad(value: number) {
    return String(value).padStart(2, '0')
}

// Standard tidsrom: neste hele kvarter og én time frem
function defaultWindow() {
    const start = new Date()
    start.setSeconds(0, 0)
    start.setMinutes(Math.floor(start.getMinutes() / 15) * 15 + 15)

    const end = new Date(start.getTime() + 60 * 60 * 1000)
    const date = start.toLocaleDateString('sv-SE') // YYYY-MM-DD
    const sameDay = end.toLocaleDateString('sv-SE') === date

    return {
        date,
        start: `${pad(start.getHours())}:${pad(start.getMinutes())}`,
        end: sameDay ? `${pad(end.getHours())}:${pad(end.getMinutes())}` : '',
    }
}

function formatDate(date: string) {
    return new Date(`${date}T00:00`).toLocaleDateString('nb-NO', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
    })
}

function PeopleIcon() {
    return (
        <svg
            width="16"
            height="16"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
            strokeLinejoin="round"
            aria-hidden="true"
        >
            <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
            <circle cx="9" cy="7" r="4" />
            <path d="M22 21v-2a4 4 0 0 0-3-3.87" />
            <path d="M16 3.13a4 4 0 0 1 0 7.75" />
        </svg>
    )
}

function StatusBadge({ status }: { status: RoomStatus }) {
    if (status === null) return null

    const labels = {
        free: 'Ledig',
        busy: 'Opptatt',
        checking: 'Sjekker…',
    }

    return (
        <span className={`room-status room-status-${status}`}>
            {labels[status]}
        </span>
    )
}

function RoomOverviewPage() {
    const { isAuthenticated } = useAuth()
    const initialWindow = useMemo(defaultWindow, [])

    const [rooms, setRooms] = useState<Room[]>([])
    const [loading, setLoading] = useState(true)
    const [loadError, setLoadError] = useState('')

    // Filtre
    const [search, setSearch] = useState('')
    const [building, setBuilding] = useState('')
    const [minCapacity, setMinCapacity] = useState(0)
    const [date, setDate] = useState(initialWindow.date)
    const [start, setStart] = useState(initialWindow.start)
    const [end, setEnd] = useState(initialWindow.end)
    const [onlyAvailable, setOnlyAvailable] = useState(false)

    // Ledighet for valgt tidsrom
    const [availableIds, setAvailableIds] = useState<Set<number> | null>(null)
    const [checking, setChecking] = useState(false)

    useEffect(() => {
        getRooms()
            .then(setRooms)
            .catch((error) =>
                setLoadError(
                    error instanceof Error
                        ? error.message
                        : 'Kunne ikke hente rom.',
                ),
            )
            .finally(() => setLoading(false))
    }, [])

    const timesValid = date !== '' && start !== '' && end !== '' && end > start

    useEffect(() => {
        if (!timesValid) {
            setAvailableIds(null)
            setChecking(false)
            return
        }

        let cancelled = false
        setChecking(true)

        const timer = setTimeout(async () => {
            try {
                const free = await getAvailableRooms(
                    `${date}T${start}`,
                    `${date}T${end}`,
                )
                if (!cancelled) {
                    setAvailableIds(new Set(free.map((room) => room.id)))
                }
            } catch {
                if (!cancelled) setAvailableIds(null)
            } finally {
                if (!cancelled) setChecking(false)
            }
        }, 300)

        return () => {
            cancelled = true
            clearTimeout(timer)
        }
    }, [date, start, end, timesValid])

    const buildings = useMemo(
        () =>
            [...new Set(rooms.map((room) => room.building))].sort((a, b) =>
                a.localeCompare(b, 'nb'),
            ),
        [rooms],
    )

    function statusFor(room: Room): RoomStatus {
        if (!timesValid) return null
        if (checking || availableIds === null) return 'checking'
        return availableIds.has(room.id) ? 'free' : 'busy'
    }

    const filteredRooms = useMemo(() => {
        const query = search.trim().toLowerCase()

        return rooms
            .filter((room) => {
                if (
                    query &&
                    !room.roomNumber.toLowerCase().includes(query) &&
                    !room.building.toLowerCase().includes(query)
                ) {
                    return false
                }
                if (building && room.building !== building) return false
                if (room.capacity < minCapacity) return false
                if (
                    onlyAvailable &&
                    timesValid &&
                    availableIds !== null &&
                    !availableIds.has(room.id)
                ) {
                    return false
                }
                return true
            })
            .sort(
                (a, b) =>
                    a.building.localeCompare(b.building, 'nb') ||
                    a.roomNumber.localeCompare(b.roomNumber, 'nb', {
                        numeric: true,
                    }),
            )
    }, [
        rooms,
        search,
        building,
        minCapacity,
        onlyAvailable,
        timesValid,
        availableIds,
    ])

    const freeCount =
        timesValid && availableIds !== null
            ? rooms.filter((room) => availableIds.has(room.id)).length
            : null

    const filtersActive =
        search !== '' || building !== '' || minCapacity > 0 || onlyAvailable

    function resetFilters() {
        setSearch('')
        setBuilding('')
        setMinCapacity(0)
        setOnlyAvailable(false)
        setDate(initialWindow.date)
        setStart(initialWindow.start)
        setEnd(initialWindow.end)
    }

    function bookingLink(room: Room) {
        const params = new URLSearchParams({ roomId: String(room.id) })
        if (timesValid) {
            params.set('date', date)
            params.set('start', start)
            params.set('end', end)
        }
        return `/bookings?${params}`
    }

    return (
        <main className="container py-5">
            <header className="mb-4">
                <span className="section-eyebrow">Rom</span>
                <h1 className="h2 fw-bold mt-1 mb-2">Romoversikt</h1>
                <p className="text-secondary mb-0">
                    Se alle grupperom, og sjekk hvilke som er ledige når du
                    trenger dem.
                </p>
            </header>

            {loadError && <div className="error-message mb-4">{loadError}</div>}

            <div className="row g-4">
                {/* Filterpanel: venstre kolonne på desktop, øverst på mobil */}
                <div className="col-12 col-lg-3">
                    <section className="card filter-panel" aria-label="Filtre">
                        <div className="card-body p-3 p-md-4">
                            <div className="row g-3">
                                <div className="col-12">
                                    <label
                                        className="form-label"
                                        htmlFor="room-search"
                                    >
                                        Søk
                                    </label>
                                    <input
                                        id="room-search"
                                        type="search"
                                        className="form-control"
                                        placeholder="Romnummer eller bygg"
                                        value={search}
                                        onChange={(e) =>
                                            setSearch(e.target.value)
                                        }
                                    />
                                </div>

                                <div className="col-6 col-lg-12">
                                    <label
                                        className="form-label"
                                        htmlFor="room-building"
                                    >
                                        Bygg
                                    </label>
                                    <select
                                        id="room-building"
                                        className="form-select"
                                        value={building}
                                        onChange={(e) =>
                                            setBuilding(e.target.value)
                                        }
                                    >
                                        <option value="">Alle bygg</option>
                                        {buildings.map((name) => (
                                            <option key={name} value={name}>
                                                {name}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div className="col-6 col-lg-12">
                                    <label
                                        className="form-label"
                                        htmlFor="room-capacity"
                                    >
                                        Plasser
                                    </label>
                                    <select
                                        id="room-capacity"
                                        className="form-select"
                                        value={minCapacity}
                                        onChange={(e) =>
                                            setMinCapacity(
                                                Number(e.target.value),
                                            )
                                        }
                                    >
                                        {capacityOptions.map((value) => (
                                            <option key={value} value={value}>
                                                {value === 0
                                                    ? 'Alle størrelser'
                                                    : `Minst ${value}`}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                            </div>

                            <hr className="my-3" />

                            <span className="filter-heading">Tidsrom</span>

                            <div className="row g-3 align-items-end">
                                <div className="col-12 col-md-4 col-lg-12">
                                    <label
                                        className="form-label"
                                        htmlFor="room-date"
                                    >
                                        Dato
                                    </label>
                                    <input
                                        id="room-date"
                                        type="date"
                                        className="form-control"
                                        min={initialWindow.date}
                                        value={date}
                                        onChange={(e) =>
                                            setDate(e.target.value)
                                        }
                                    />
                                </div>

                                <div className="col-6 col-md-4 col-lg-6">
                                    <label
                                        className="form-label"
                                        htmlFor="room-start"
                                    >
                                        Fra
                                    </label>
                                    <input
                                        id="room-start"
                                        type="time"
                                        step={900}
                                        className="form-control"
                                        value={start}
                                        onChange={(e) =>
                                            setStart(e.target.value)
                                        }
                                    />
                                </div>

                                <div className="col-6 col-md-4 col-lg-6">
                                    <label
                                        className="form-label"
                                        htmlFor="room-end"
                                    >
                                        Til
                                    </label>
                                    <input
                                        id="room-end"
                                        type="time"
                                        step={900}
                                        className="form-control"
                                        value={end}
                                        onChange={(e) => setEnd(e.target.value)}
                                    />
                                </div>

                                <div className="col-12">
                                    <div className="form-check form-switch filter-switch">
                                        <input
                                            id="only-available"
                                            type="checkbox"
                                            role="switch"
                                            className="form-check-input"
                                            checked={onlyAvailable}
                                            disabled={!timesValid}
                                            onChange={(e) =>
                                                setOnlyAvailable(
                                                    e.target.checked,
                                                )
                                            }
                                        />
                                        <label
                                            className="form-check-label"
                                            htmlFor="only-available"
                                        >
                                            Vis bare ledige rom
                                        </label>
                                    </div>
                                </div>
                            </div>

                            {start !== '' && end !== '' && end <= start && (
                                <p className="text-danger small mt-2 mb-0">
                                    Sluttid må være etter starttid.
                                </p>
                            )}
                        </div>
                    </section>
                </div>

                {/* Rom: høyre kolonne på desktop */}
                <div className="col-12 col-lg-9">
                    {/* Resultatlinje */}
                    <div className="results-bar">
                        <p className="mb-0">
                            <strong>{filteredRooms.length}</strong> rom
                            {timesValid && freeCount !== null && (
                                <span className="text-secondary">
                                    {' '}
                                    · {freeCount} av {rooms.length} ledige{' '}
                                    {formatDate(date)}, {start}–{end}
                                </span>
                            )}
                        </p>

                        {filtersActive && (
                            <button
                                type="button"
                                className="btn btn-link btn-sm text-secondary text-decoration-none p-0"
                                onClick={resetFilters}
                            >
                                Nullstill filtre
                            </button>
                        )}
                    </div>

                    {/* Romkort */}
                    {loading ? (
                        <div className="room-grid">
                            {[0, 1, 2, 3, 4, 5].map((i) => (
                                <div
                                    key={i}
                                    className="room-card room-skeleton"
                                />
                            ))}
                        </div>
                    ) : filteredRooms.length === 0 ? (
                        <div className="empty-state">
                            <p className="fw-semibold text-body mb-1">
                                Ingen rom passer filtrene
                            </p>
                            <p className="mb-3">
                                Prøv et annet tidspunkt eller færre filtre.
                            </p>
                            <button
                                type="button"
                                className="btn btn-light btn-sm"
                                onClick={resetFilters}
                            >
                                Nullstill filtre
                            </button>
                        </div>
                    ) : (
                        <div className="room-grid">
                            {filteredRooms.map((room) => {
                                const status = statusFor(room)

                                return (
                                    <article
                                        key={room.id}
                                        className={`room-card ${status === 'busy' ? 'is-busy' : ''}`}
                                    >
                                        <div className="room-card-top">
                                            <div className="min-w-0">
                                                <h2 className="room-card-number">
                                                    {room.roomNumber}
                                                </h2>
                                                <p className="room-card-building">
                                                    {room.building}
                                                </p>
                                            </div>
                                            <StatusBadge status={status} />
                                        </div>

                                        <p className="room-card-capacity">
                                            <PeopleIcon />
                                            {room.capacity}{' '}
                                            {room.capacity === 1
                                                ? 'plass'
                                                : 'plasser'}
                                        </p>

                                        <div className="room-card-footer">
                                            {status === 'busy' ? (
                                                <span className="room-card-note">
                                                    Opptatt i valgt tidsrom
                                                </span>
                                            ) : !isAuthenticated ? (
                                                <Link
                                                    to="/login"
                                                    className="btn btn-sm btn-light w-100"
                                                >
                                                    Logg inn for å booke
                                                </Link>
                                            ) : (
                                                <Link
                                                    to={bookingLink(room)}
                                                    className={`btn btn-sm w-100 ${status === 'free' ? 'btn-primary' : 'btn-outline-primary'}`}
                                                >
                                                    {status === 'free'
                                                        ? `Book ${start}–${end}`
                                                        : 'Book dette rommet'}
                                                </Link>
                                            )}
                                        </div>
                                    </article>
                                )
                            })}
                        </div>
                    )}
                </div>
            </div>
        </main>
    )
}

export default RoomOverviewPage