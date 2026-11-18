function pad(value: number) {
    return String(value).padStart(2, '0')
}

export function getDefaultBookingWindow() {
    const start = new Date()

    start.setSeconds(0, 0)
    start.setMinutes(
        Math.floor(start.getMinutes() / 15) * 15 + 15
    )

    const end = new Date(
        start.getTime() + 60 * 60 * 1000
    )

    const date =
        start.toLocaleDateString('sv-SE')

    const sameDay =
        end.toLocaleDateString('sv-SE') === date

    return {
        date,
        start: `${pad(start.getHours())}:${pad(start.getMinutes())}`,
        end: sameDay
            ? `${pad(end.getHours())}:${pad(end.getMinutes())}`
            : '',
    }
}