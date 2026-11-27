import { formatShortMonth } from '../utils/dateFormatting'

interface DateBlockProps {
    iso: string
    compact?: boolean
}

/** Displays a booking date in either the regular or compact date tile. */
export default function DateBlock({ iso, compact = false }: DateBlockProps) {
    const date = new Date(iso)

    return (
        <div className={`date-block${compact ? ' date-block-compact' : ''}`}>
            <span className="date-block-month">{formatShortMonth(date)}</span>
            <span className="date-block-day">{date.getDate()}</span>
        </div>
    )
}
