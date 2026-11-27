import { Link } from 'react-router-dom'
import type { Room } from '../../types/Room'
import AmenityBadges from '../AmenityBadges'

export type RoomStatus = 'free' | 'busy' | 'checking' | 'error' | null

interface RoomCardProps {
    room: Room
    status: RoomStatus
    isAuthenticated: boolean
    bookingHref: string
    timeLabel: string
}

/** Renders the capacity icon. */
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

/** Presents the current availability status. */
function StatusBadge({ status }: { status: RoomStatus }) {
    if (status === null) return null

    const labels = {
        free: 'Ledig',
        busy: 'Opptatt',
        checking: 'Sjekker…',
        error: 'Ukjent',
    }

    return (
        <span className={`room-status room-status-${status}`}>
            {labels[status]}
        </span>
    )
}

/** Displays one room and the action appropriate to its availability. */
export default function RoomCard({ room, status, isAuthenticated, bookingHref, timeLabel }: RoomCardProps) {
    return (
        <article className={`room-card ${status === 'busy' ? 'is-busy' : ''}`}>
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

            <AmenityBadges amenities={room.amenities} className="mb-3" />

            <div className="room-card-footer">
                {status === 'checking' || status === 'error' ? (
                    <span className="room-card-note">{status === 'checking' ? 'Sjekker ledighet…' : 'Ledighet ukjent'}</span>
                ) : status === 'busy' ? (
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
                        to={bookingHref}
                        className={`btn btn-sm w-100 ${status === 'free' ? 'btn-primary' : 'btn-outline-primary'}`}
                    >
                        {status === 'free'
                            ? `Book ${timeLabel}`
                            : 'Book dette rommet'}
                    </Link>
                )}
            </div>
        </article>
    )
}
