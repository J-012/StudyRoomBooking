import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'

import {
    getOpenSessions,
    joinSession,
    leaveSession,
} from '../api/bookingsApi'

import type { Booking } from '../types/Booking'

import SessionCard from '../components/SessionCard'
import { errorText } from '../utils/errorHandling'

import './SessionsPage.css'

/** Lists open study sessions and handles participation. */
function SessionsPage() {
    const [sessions, setSessions] = useState<Booking[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    // Search filters
    const [search, setSearch] = useState('')
    const [onlyWithSpace, setOnlyWithSpace] = useState(false)

    // Tracks the session being joined or left.
    const [busyId, setBusyId] = useState<number | null>(null)

    /** Refreshes the list and participant counts from the API. */
    async function loadSessions() {
        try {
            setSessions(await getOpenSessions())
        } catch (error) {
            setError(errorText(error, 'Kunne ikke hente økter.'))
        } finally {
            setLoading(false)
        }
    }

    // Initial load is cancelled when the page unmounts.
    useEffect(() => {
        let cancelled = false
        getOpenSessions().then(data => {
            if (!cancelled) setSessions(data)
        }).catch(error => {
            if (!cancelled) setError(errorText(error, 'Kunne ikke hente økter.'))
        }).finally(() => {
            if (!cancelled) setLoading(false)
        })
        return () => { cancelled = true }
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

    /** Joins or leaves a session and reloads its server-side state. */
    async function changeParticipation(
        session: Booking,
        action: (id: number) => Promise<void>,
        fallback: string
    ) {
        if (busyId !== null) return
        try {
            setBusyId(session.id)
            setError('')

            await action(session.id)
        } catch (error) {
            setError(errorText(error, fallback))
        } finally {
            // Reload even after a failure: another student may have filled the last seat.
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
                                    busy={busyId !== null}
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
