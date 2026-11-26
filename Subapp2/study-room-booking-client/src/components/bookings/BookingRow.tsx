import type { Booking } from '../../types/Booking'
import { formatShortMonth, formatTime, formatWeekday } from '../../utils/dateFormatting'

/** Displays the date in a compact booking-list tile. */
function DateBlock({ iso }: { iso: string }) {
    const date = new Date(iso)
    const month = formatShortMonth(date)

    return (
        <div className="date-block">
            <span className="date-block-month">{month}</span>
            <span className="date-block-day">{date.getDate()}</span>
        </div>
    )
}

interface BookingRowProps {
    booking: Booking
    busy: boolean
    isEditing: boolean
    isPast: boolean
    onEdit: () => void
    onDelete: () => void
}

/** Displays one booking and its permitted management actions. */
export default function BookingRow({
    booking,
    busy,
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
                            disabled={busy}
                            onClick={onEdit}
                        >
                            Endre
                        </button>
                    )}
                    <button
                        type="button"
                        className="btn btn-sm btn-link text-danger text-decoration-none"
                        disabled={busy}
                        onClick={onDelete}
                    >
                        Slett
                    </button>
                </div>
            )}
        </li>
    )
}
