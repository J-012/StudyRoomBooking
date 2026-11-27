/** Formats a date as YYYY-MM-DD in local time for HTML date inputs. */
export function formatLocalDate(date: Date = new Date()): string {
    const year = String(date.getFullYear()).padStart(4, '0')
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')

    return `${year}-${month}-${day}`
}

/** Formats an API date-time as a Norwegian 24-hour clock time. */
export function formatTime(iso: string): string {
    return new Date(iso).toLocaleTimeString('nb-NO', {
        hour: '2-digit',
        minute: '2-digit',
    })
}

/** Returns the Norwegian weekday of an API date-time. */
export function formatWeekday(iso: string): string {
    return new Date(iso).toLocaleDateString('nb-NO', { weekday: 'long' })
}

/** Formats a date or date-time as a Norwegian weekday, day and month. */
export function formatDay(value: string): string {
    // Date-only strings must be interpreted as local time, not UTC midnight.
    const date = new Date(value.length === 10 ? `${value}T00:00` : value)

    return date.toLocaleDateString('nb-NO', {
        weekday: 'long',
        day: 'numeric',
        month: 'long',
    })
}

/** Returns an abbreviated month without the trailing period for date cards. */
export function formatShortMonth(date: Date): string {
    return date.toLocaleDateString('nb-NO', { month: 'short' }).replace('.', '')
}
