import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'

import {
    getOpenSessions,
    joinSession,
    leaveSession,
} from '../api/bookingsApi'

import type { Booking } from '../types/Booking'

import './SessionsPage.css'

function errorText(error: unknown, fallback: string) {
    return error instanceof Error ? error.message : fallback
}

function formatDay(iso: string) {
    return new Date(iso).toLocaleDateString('nb-NO', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
    })
}

function formatTime(iso: string) {
    return new Date(iso).toLocaleTimeString('nb-NO', {
        hour: '2-digit',
        minute: '2-digit',
    })
}

interface SessionCardProps {
    session: Booking
    busy: boolean
    onJoin: () => void
    onLeave: () => void
}

function SessionCard({ session, busy, onJoin, onLeave }: SessionCardProps) {
    const isFull = session.attendeeCount >= session.capacity

    return (
        <article className="card session-card h-100">
            <div className="card-body p-4 d-flex flex-column">
                <div className="d-flex justify-content-between align-items-start gap-2 mb-2">
                    <h2 className="h5 fw-bold mb-0">{session.subject}</h2>

                    <span
                        className={`badge rounded-pill session-seats ${isFull ? 'is-full' : ''}`}
                    >
                        {session.attendeeCount} av {session.capacity} plasser
                    </span>
                </div>

                <p className="text-secondary mb-3">{session.topic}</p>

                <ul className="session-facts">
                    <li>
                        <span className="text-capitalize">
                            {formatDay(session.startTime)}
                        </span>
                        , {formatTime(session.startTime)}–
                        {formatTime(session.endTime)}
                    </li>
                    <li>
                        {session.roomNumber} · {session.building}
                    </li>
                    <li>Arrangør: {session.organizerName}</li>
                </ul>

                <div className="mt-auto pt-3">
                    {session.isOrganizer ? (
                        <Link
                            to="/bookings"
                            className="btn btn-light btn-sm w-100"
                        >
                            Din økt – administrer
                        </Link>
                    ) : session.isParticipant ? (
                        <button
                            type="button"
                            className="btn btn-outline-secondary btn-sm w-100"
                            disabled={busy}
                            onClick={onLeave}
                        >
                            {busy ? 'Vent…' : 'Du er med – forlat økten'}
                        </button>
                    ) : isFull ? (
                        <button
                            type="button"
                            className="btn btn-light btn-sm w-100"
                            disabled
                        >
                            Fullt
                        </button>
                    ) : (
                        <button
                            type="button"
                            className="btn btn-primary btn-sm w-100"
                            disabled={busy}
                            onClick={onJoin}
                        >
                            {busy ? 'Vent…' : 'Bli med'}
                        </button>
                    )}
                </div>
            </div>
        </article>
    )
}

function SessionsPage() {
    const [sessions, setSessions] = useState<Booking[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    // Filtre
    const [search, setSearch] = useState('')
    const [onlyWithSpace, setOnlyWithSpace] = useState(false)

    // Id-en til økten som har et bli med/forlat-kall på gang
    const [busyId, setBusyId] = useState<number | null>(null)

    async function loadSessions() {
        try {
            setSessions(await getOpenSessions())
        } catch (error) {
            setError(errorText(error, 'Kunne ikke hente økter.'))
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadSessions()
    }, [])

    const filteredSessions = useMemo(() => {
        const query = search.trim().toLowerCase()

        return sessions.filter((session) => {
            if (
                query &&
                !session.subject.toLowerCase().includes(query) &&
                !session.topic.toLowerCase().includes(query)
            ) {
                return false
            }

            if (
                onlyWithSpace &&
                session.attendeeCount >= session.capacity
            ) {
                return false
            }

            return true
        })
    }, [sessions, search, onlyWithSpace])

    async function changeParticipation(
        session: Booking,
        action: (id: number) => Promise<void>,
        fallback: string
    ) {
        try {
            setBusyId(session.id)
            setError('')

            await action(session.id)
        } catch (error) {
            setError(errorText(error, fallback))
        } finally {
            // Hent listen på nytt uansett, så antall plasser stemmer
            // også når noen andre rakk å ta den siste plassen.
            await loadSessions()
            setBusyId(null)
        }
    }

    return (
        <main className="container py-5">
            <header className="mb-4">
                <span className="section-eyebrow">Studieøkter</span>
                <h1 className="h2 fw-bold mt-1 mb-2">Åpne økter</h1>
                <p className="text-secondary mb-0">
                    Finn en økt i faget ditt og bli med, så lenge det er
                    ledig plass i rommet.
                </p>
            </header>

            {error && <div className="error-message mb-4">{error}</div>}

            <section className="card mb-4" aria-label="Filtre">
                <div className="card-body p-3 p-md-4">
                    <div className="row g-3 align-items-end">
                        <div className="col-12 col-md-7">
                            <label
                                className="form-label"
                                htmlFor="session-search"
                            >
                                Søk
                            </label>
                            <input
                                id="session-search"
                                type="search"
                                className="form-control"
                                placeholder="Fag eller tema, f.eks. DATA2410"
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </div>

                        <div className="col-12 col-md-5">
                            <div className="form-check form-switch mb-2">
                                <input
                                    id="only-with-space"
                                    type="checkbox"
                                    role="switch"
                                    className="form-check-input"
                                    checked={onlyWithSpace}
                                    onChange={(e) =>
                                        setOnlyWithSpace(e.target.checked)
                                    }
                                />
                                <label
                                    className="form-check-label"
                                    htmlFor="only-with-space"
                                >
                                    Vis bare økter med ledig plass
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {loading ? (
                <p className="text-secondary">Henter økter…</p>
            ) : sessions.length === 0 ? (
                <div className="card">
                    <div className="card-body text-center py-5">
                        <p className="fw-semibold mb-1">
                            Ingen kommende økter
                        </p>
                        <p className="text-secondary mb-3">
                            Vær den første til å booke et rom.
                        </p>
                        <Link
                            to="/bookings"
                            className="btn btn-primary btn-sm px-3"
                        >
                            Book et rom
                        </Link>
                    </div>
                </div>
            ) : filteredSessions.length === 0 ? (
                <div className="card">
                    <div className="card-body text-center py-5">
                        <p className="fw-semibold mb-1">
                            Ingen økter passer filtrene
                        </p>
                        <p className="text-secondary mb-0">
                            Prøv et annet søk.
                        </p>
                    </div>
                </div>
            ) : (
                <>
                    <p className="text-secondary small mb-3">
                        {filteredSessions.length}{' '}
                        {filteredSessions.length === 1 ? 'økt' : 'økter'}
                    </p>

                    <div className="row g-4">
                        {filteredSessions.map((session) => (
                            <div
                                key={session.id}
                                className="col-12 col-md-6 col-lg-4"
                            >
                                <SessionCard
                                    session={session}
                                    busy={busyId === session.id}
                                    onJoin={() =>
                                        changeParticipation(
                                            session,
                                            joinSession,
                                            'Kunne ikke bli med i økten.'
                                        )
                                    }
                                    onLeave={() =>
                                        changeParticipation(
                                            session,
                                            leaveSession,
                                            'Kunne ikke forlate økten.'
                                        )
                                    }
                                />
                            </div>
                        ))}
                    </div>
                </>
            )}
        </main>
    )
}

export default SessionsPage

