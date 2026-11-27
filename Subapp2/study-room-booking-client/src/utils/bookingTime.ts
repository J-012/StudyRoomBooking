import { formatLocalDate } from './dateFormatting'

/** Formats a local time as HH:mm for HTML time inputs. */
function formatInputTime(date: Date): string {
    const hours = String(date.getHours()).padStart(2, '0')
    const minutes = String(date.getMinutes()).padStart(2, '0')

    return `${hours}:${minutes}`
}

/** Returns the next quarter-hour and a one-hour default booking window. */
export function getDefaultBookingWindow() {
    const start = new Date()
    start.setSeconds(0, 0)
    start.setMinutes(Math.floor(start.getMinutes() / 15) * 15 + 15)

    const end = new Date(start.getTime() + 60 * 60 * 1000)
    const date = formatLocalDate(start)

    return {
        date,
        start: formatInputTime(start),
        // The form supports a single date, so do not prefill a cross-midnight end time.
        end: formatLocalDate(end) === date ? formatInputTime(end) : '',
    }
}

/** Checks that all time fields are present and the end follows the start. */
export function hasOrderedBookingTimes(date: string, start: string, end: string): boolean {
    const timePattern = /^(?:[01]\d|2[0-3]):[0-5]\d$/
    if (!/^\d{4}-\d{2}-\d{2}$/.test(date) || !timePattern.test(start) || !timePattern.test(end)) return false

    // Reject normalized dates such as February 30 rather than silently accepting them.
    const day = new Date(`${date}T00:00`)
    return !Number.isNaN(day.getTime()) && formatLocalDate(day) === date && end > start
}

/** Mirrors basic server time rules for immediate form feedback. The server remains authoritative. */
export function validateBookingTimes(date: string, start: string, end: string): string {
    if (!hasOrderedBookingTimes(date, start, end)) return 'Velg en gyldig dato og sluttid etter starttid.'

    const startDate = new Date(`${date}T${start}`)
    const endDate = new Date(`${date}T${end}`)
    const duration = (endDate.getTime() - startDate.getTime()) / 60_000
    if (duration < 15 || duration > 180) return 'En booking må vare mellom 15 minutter og 3 timer.'
    if (startDate.getTime() < Date.now()) return 'Starttid kan ikke være i fortiden.'
    return ''
}
