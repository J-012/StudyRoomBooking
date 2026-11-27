import { useEffect, useMemo, useState } from 'react'
import { getAvailableRooms } from '../api/roomsApi'
import type { Room } from '../types/Room'
import { hasOrderedBookingTimes } from '../utils/bookingTime'
import { errorText } from '../utils/errorHandling'

interface AvailabilityQuery {
    date: string
    start: string
    end: string
    excludeId?: number
    revision: number
}

interface AvailabilityResult {
    query: AvailabilityQuery
    rooms: Room[]
    error: string
}

const noRooms: Room[] = []

/** Checks availability for a time window and ignores outdated responses. */
export function useAvailableRooms(date: string, start: string, end: string, excludeId?: number) {
    const [result, setResult] = useState<AvailabilityResult | null>(null)
    const [revision, setRevision] = useState(0)
    const valid = hasOrderedBookingTimes(date, start, end)

    // A new query identity invalidates previous results, even if the user returns to an earlier time.
    const query = useMemo(() => ({ date, start, end, excludeId, revision }), [date, start, end, excludeId, revision])

    useEffect(() => {
        if (!valid) return
        let cancelled = false

        // Debounce typing and ignore responses belonging to a previous time window.
        const timer = setTimeout(async () => {
            try {
                const rooms = await getAvailableRooms(`${query.date}T${query.start}`, `${query.date}T${query.end}`, query.excludeId)
                if (!cancelled) setResult({ query, rooms, error: '' })
            } catch (error) {
                if (!cancelled) setResult({ query, rooms: [], error: errorText(error, 'Kunne ikke hente ledige rom.') })
            }
        }, 300)

        return () => {
            cancelled = true
            clearTimeout(timer)
        }
    }, [query, valid])

    const current = valid && result?.query === query ? result : null

    /** Invalidates the cached result after a booking changes. */
    function refresh() {
        setRevision(value => value + 1)
    }

    return {
        rooms: current?.rooms ?? noRooms,
        loading: valid && current === null,
        error: current?.error ?? '',
        ready: current !== null && !current.error,
        valid,
        refresh,
    }
}
