import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

import { getBookings } from '../api/bookingsApi'
import { useAuth } from '../context/useAuth'
import type { Booking } from '../types/Booking'

import { formatTime } from '../utils/dateFormatting'
import DateBlock from '../components/DateBlock'

import './HomePage.css'

const steps = [
    'Velg tidspunkt',
    'Finn et ledig rom',
    'Book og møt opp',
]

/** Shows the next three bookings for the signed-in student. */
function UpcomingSessions() {
    const [items, setItems] = useState<Booking[] | null>(null)
    const [failed, setFailed] = useState(false)

    useEffect(() => {
        let cancelled = false

        getBookings()
            .then((data) => {
                if (cancelled) return
                const now = Date.now()
                setItems(
                    data
                        .filter((b) => new Date(b.endTime).getTime() >= now)
                        .sort(
                            (a, b) =>
                                new Date(a.startTime).getTime() -
                                new Date(b.startTime).getTime()
                        )
                        .slice(0, 3)
                )
            })
            .catch(() => {
                if (!cancelled) setFailed(true)
            })

        return () => {
            cancelled = true
        }
    }, [])

    return (
        <div className="hp-panel">
            <div className="hp-panel-head">
                <span className="hp-panel-title">Dine neste økter</span>
                <Link to="/bookings" className="hp-panel-link">
                    Se alle
                </Link>
            </div>

            {failed ? (
                <p className="hp-panel-empty">
                    Kunne ikke hente bookingene dine akkurat nå.
                </p>
            ) : items === null ? (
                <ul className="hp-sessions">
                    {[0, 1, 2].map((i) => (
                        <li key={i} className="hp-session hp-skeleton loading-skeleton" aria-hidden="true" />
                    ))}
                </ul>
            ) : items.length === 0 ? (
                <div className="hp-panel-empty">
                    <p className="mb-3">Du har ingen kommende økter.</p>
                    <Link to="/bookings" className="btn btn-primary btn-sm px-3">
                        Book et rom
                    </Link>
                </div>
            ) : (
                <ul className="hp-sessions">
                    {items.map((booking) => (
                        <li key={booking.id} className="hp-session">
                            <DateBlock iso={booking.startTime} compact />
                            <div className="min-w-0">
                                <p className="hp-session-title">
                                    {booking.subject}
                                </p>
                                <p className="hp-session-meta">
                                    {formatTime(booking.startTime)}–
                                    {formatTime(booking.endTime)} ·{' '}
                                    {booking.roomNumber}, {booking.building}
                                </p>
                            </div>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    )
}

/** Illustrates the booking flow for guests without using live data. */
function BookingPreview() {
    const rooms = [
        { number: 'A2-14', building: 'Bygg A', selected: false },
        { number: 'B1-05', building: 'Bygg B', selected: true },
        { number: 'B3-22', building: 'Bygg B', selected: false },
    ]

    return (
        <div className="hp-panel" aria-hidden="true">
            <div className="hp-panel-head">
                <span className="hp-panel-title">Ny booking</span>
                <span className="hp-preview-tag">Eksempel</span>
            </div>

            <div className="hp-preview-fields">
                <div className="hp-preview-field">
                    <span>Dato</span>
                    <strong>Tirsdag 18. nov.</strong>
                </div>
                <div className="hp-preview-field">
                    <span>Tid</span>
                    <strong>14:00–16:00</strong>
                </div>
            </div>

            <span className="hp-preview-label">Ledige rom (3)</span>

            <div className="hp-preview-rooms">
                {rooms.map((room) => (
                    <div
                        key={room.number}
                        className={`hp-preview-room ${room.selected ? 'is-selected' : ''}`}
                    >
                        <strong>{room.number}</strong>
                        <span>{room.building}</span>
                    </div>
                ))}
            </div>

            <div className="hp-preview-button">Book rom</div>
        </div>
    )
}

/** Landing page with role-specific navigation and booking information. */
function HomePage() {
    const { isAuthenticated, isAdmin, fullName } = useAuth()
    const isStudent = isAuthenticated && !isAdmin

    return (
        <main>
            <section className="hp-hero">
                <div className="container">
                    <div className="row align-items-center g-5">
                        <div className="col-lg-6">
                            <span className="hp-eyebrow">
                                {isAuthenticated
                                    ? `Hei, ${fullName}`
                                    : 'Grupperom for studenter'}
                            </span>

                            <h1 className="hp-title">
                                Finn et ledig grupperom
                            </h1>
                            <div className="d-flex flex-wrap gap-2">
                                {!isAuthenticated && (
                                    <>
                                        <Link
                                            to="/login"
                                            className="btn btn-primary btn-lg px-4"
                                        >
                                            Logg inn
                                        </Link>
                                        <Link
                                            to="/room-overview"
                                            className="btn btn-outline-dark btn-lg px-4"
                                        >
                                            Se romoversikt
                                        </Link>
                                    </>
                                )}

                                {isStudent && (
                                    <>
                                        <Link
                                            to="/room-overview"
                                            className="btn btn-primary btn-lg px-4"
                                        >
                                            Finn rom
                                        </Link>

                                        <Link
                                            to="/bookings"
                                            className="btn btn-outline-dark btn-lg px-4"
                                        >
                                            Mine bookinger
                                        </Link>
                                    </>
                                )}

                                {isAdmin && (
                                    <>
                                        <Link
                                            to="/rooms"
                                            className="btn btn-primary btn-lg px-4"
                                        >
                                            Administrer rom
                                        </Link>
                                        <Link
                                            to="/room-overview"
                                            className="btn btn-outline-dark btn-lg px-4"
                                        >
                                            Se romoversikt
                                        </Link>
                                    </>
                                )}
                            </div>
                        </div>

                        <div className="col-lg-6">
                            <div className="hp-visual">
                                {isStudent ? (
                                    <UpcomingSessions />
                                ) : (
                                    <BookingPreview />
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section className="container pb-5">
                <div className="text-center mb-4">
                    <span className="section-eyebrow">Slik fungerer det</span>
                    <h2 className="h3 fw-bold mt-1">Tre steg til et ledig rom</h2>
                </div>

                <ol className="hp-steps">
                    {steps.map((step, index) => (
                        <li key={step} className="hp-step">
                            <span className="hp-step-number">{index + 1}</span>
                            <h3 className="hp-step-title">{step}</h3>
                        </li>
                    ))}
                </ol>

                {isAuthenticated ? (
                    <div className="hp-band">
                        <div>
                            <h2 className="h4 fw-bold mb-1">
                                Leter du etter et bestemt rom?
                            </h2>
                            <p className="text-secondary mb-0">
                                Søk etter bygg og antall plasser, og se hva som er
                                ledig akkurat nå.
                            </p>
                        </div>
                        <Link
                            to="/room-overview"
                            className="btn btn-dark px-4 text-nowrap"
                        >
                            Åpne romoversikten
                        </Link>
                    </div>
                ) : (
                    <div className="hp-band">
                        <div>
                            <h2 className="h4 fw-bold mb-1">
                                Klar for neste studieøkt?
                            </h2>
                            <p className="text-secondary mb-0">
                                Lag en bruker for å booke rom. Har du allerede en?{' '}
                                <Link to="/login" className="hp-inline-link">
                                    Logg inn
                                </Link>
                            </p>
                        </div>
                        <Link
                            to="/register"
                            className="btn btn-dark px-4 text-nowrap"
                        >
                            Kom i gang
                        </Link>
                    </div>
                )}
            </section>
        </main>
    )
}

export default HomePage
