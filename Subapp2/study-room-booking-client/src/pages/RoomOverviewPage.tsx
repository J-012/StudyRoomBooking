import { useMemo, useState } from 'react'

import { getRooms } from '../api/roomsApi'
import { getBuildings } from '../api/buildingsApi'
import { useAuth } from '../context/useAuth'
import type { Room } from '../types/Room'
import { getDefaultBookingWindow } from '../utils/bookingTime'
import { formatDay } from '../utils/dateFormatting'
import { useAsyncData } from '../hooks/useAsyncData'
import { useAvailableRooms } from '../hooks/useAvailableRooms'
import RoomCard from '../components/rooms/RoomCard'
import type { RoomStatus } from '../components/rooms/RoomCard'
import RoomFilterPanel from '../components/rooms/RoomFilterPanel'
import type { RoomFilters } from '../components/rooms/RoomFilterPanel'

import './RoomOverviewPage.css'

/** Loads the two collections needed by the overview and its building filter. */
async function loadRoomOverview() {
    const [rooms, buildings] = await Promise.all([getRooms(), getBuildings()])
    return { rooms, buildings }
}

/** Creates a fresh set of room filters with the default booking time. */
function createDefaultFilters(): RoomFilters {
    return { search: '', buildingId: 0, minCapacity: 0, onlyAvailable: false, ...getDefaultBookingWindow() }
}

/** Displays rooms with combined filters and live availability. */
function RoomOverviewPage() {
    const { isAuthenticated } = useAuth()
    const [filters, setFilters] = useState(createDefaultFilters)
    const [minDate] = useState(() => filters.date)
    const { search, buildingId, minCapacity, date, start, end, onlyAvailable } = filters
    const { data, loading, error: loadError, refresh } = useAsyncData(loadRoomOverview, 'Kunne ikke hente rom og bygg.')

    const availability = useAvailableRooms(date, start, end)
    const { valid: timesValid, loading: checking, rooms: freeRooms, ready } = availability
    const availableIds = useMemo(
        () => ready ? new Set(freeRooms.map(room => room.id)) : null,
        [ready, freeRooms]
    )

    /** Converts the current availability result into a room status. */
    function statusFor(room: Room): RoomStatus {
        if (!timesValid) return null
        if (checking) return 'checking'
        if (availability.error || availableIds === null) return 'error'
        return availableIds.has(room.id) ? 'free' : 'busy'
    }

    // Never show unverified rooms while the user requests available rooms only.
    const waitingForAvailability = onlyAvailable && availableIds === null
    const filteredRooms = useMemo(() => {
        if (waitingForAvailability) return []
        const query = search.trim().toLowerCase()

        return (data?.rooms ?? [])
            .filter(room => {
                if (query && !room.roomNumber.toLowerCase().includes(query) &&
                    !room.building.toLowerCase().includes(query)) return false
                if (buildingId > 0 && room.buildingId !== buildingId) return false
                if (room.capacity < minCapacity) return false
                if (onlyAvailable && !availableIds?.has(room.id)) return false
                return true
            })
            .sort((a, b) => a.building.localeCompare(b.building, 'nb') ||
                a.roomNumber.localeCompare(b.roomNumber, 'nb', { numeric: true }))
    }, [data, search, buildingId, minCapacity, onlyAvailable, availableIds, waitingForAvailability])

    const rooms = data?.rooms ?? []
    const freeCount = availableIds === null ? null : rooms.filter(room => availableIds.has(room.id)).length
    const filtersActive = search !== '' || buildingId > 0 || minCapacity > 0 || onlyAvailable

    /** Changes one filter without mutating the remaining values. */
    function updateFilter<K extends keyof RoomFilters>(key: K, value: RoomFilters[K]) {
        setFilters(current => ({ ...current, [key]: value }))
    }

    /** Resets all filters and selects a fresh default booking time. */
    function resetFilters() {
        setFilters(createDefaultFilters())
    }

    /** Carries the selected room and time window to the booking form. */
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

            {availability.error && <div className="error-message mb-4" role="alert">{availability.error} <button type="button" className="btn btn-sm btn-light" onClick={availability.refresh}>Prøv igjen</button></div>}

            <div className="row g-4">
                {/* Filter panel */}
                <div className="col-12 col-lg-3">
                    <RoomFilterPanel
                        filters={filters}
                        buildings={data?.buildings ?? []}
                        minDate={minDate}
                        timesValid={timesValid}
                        availabilityReady={ready}
                        updateFilter={updateFilter}
                    />
                </div>

                {/* Room results */}
                <div className="col-12 col-lg-9">
                    {/* Results summary */}
                    <div className="results-bar">
                        <p className="mb-0">
                            {loading ? 'Henter rom…' : loadError ? 'Rom kunne ikke hentes' : waitingForAvailability ? 'Ledighet ikke bekreftet' : (
                                <><strong>{filteredRooms.length}</strong> rom</>
                            )}
                            {!loading && !loadError && timesValid && freeCount !== null && (
                                <span className="text-secondary">
                                    {' '}
                                    · {freeCount} av {rooms.length} ledige{' '}
                                    {formatDay(date)}, {start}–{end}
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

                    {/* Room cards */}
                    {loading ? (
                        <div className="room-grid">
                            {[0, 1, 2, 3, 4, 5].map((i) => (
                                <div
                                    key={i}
                                    className="room-card room-skeleton loading-skeleton"
                                    aria-hidden="true"
                                />
                            ))}
                        </div>
                    ) : loadError ? (
                        <div className="error-message" role="alert">
                            {loadError}{' '}
                            <button type="button" className="btn btn-sm btn-light" onClick={refresh}>Prøv igjen</button>
                        </div>
                    ) : waitingForAvailability ? (
                        <p className="text-secondary" role="status">
                            {!timesValid ? 'Velg et gyldig tidsrom for å se ledige rom.' : checking ? 'Sjekker ledige rom…' : 'Kunne ikke bekrefte hvilke rom som er ledige.'}
                        </p>
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
                            {filteredRooms.map(room => (
                                <RoomCard
                                    key={room.id}
                                    room={room}
                                    status={statusFor(room)}
                                    isAuthenticated={isAuthenticated}
                                    bookingHref={bookingLink(room)}
                                    timeLabel={`${start}–${end}`}
                                />
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </main>
    )
}

export default RoomOverviewPage
