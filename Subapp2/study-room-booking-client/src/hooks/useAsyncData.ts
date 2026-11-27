import { useEffect, useState } from 'react'
import { errorText } from '../utils/errorHandling'

interface LoadResult<T> {
    revision: number
    data: T | null
    error: string
}

/** Loads data on mount and refresh, ignoring obsolete results. Pass a stable load function. */
export function useAsyncData<T>(load: () => Promise<T>, fallback: string) {
    const [revision, setRevision] = useState(0)
    const [result, setResult] = useState<LoadResult<T> | null>(null)

    useEffect(() => {
        let cancelled = false
        load().then(data => {
            if (!cancelled) setResult({ revision, data, error: '' })
        }).catch(error => {
            if (!cancelled) {
                setResult({ revision, data: null, error: errorText(error, fallback) })
            }
        })
        return () => { cancelled = true }
    }, [load, fallback, revision])

    // Previous data must not appear as a current result during a refresh.
    const current = result?.revision === revision ? result : null

    /** Requests a new load without treating a refresh failure as a mutation failure. */
    function refresh() {
        setRevision(value => value + 1)
    }

    return {
        data: current?.data ?? null,
        loading: current === null,
        error: current?.error ?? '',
        refresh,
    }
}
