import { Link } from 'react-router-dom'
import type { Booking } from '../types/Booking'
import { formatDay, formatTime } from '../utils/dateFormatting'

interface SessionCardProps {
    session: Booking
    busy: boolean
    onJoin: () => void
    onLeave: () => void
}

/** Displays a study session and its participation controls. */
export default function SessionCard({ session, busy, onJoin, onLeave }: SessionCardProps) {
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
