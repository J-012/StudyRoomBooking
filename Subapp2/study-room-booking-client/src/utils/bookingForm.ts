import { getDefaultBookingWindow, hasOrderedBookingTimes } from './bookingTime'

/** Local form fields; date and time are combined when sending the API request. */
export interface BookingForm {
    subject: string
    topic: string
    roomId: number
    date: string // YYYY-MM-DD
    start: string // HH:mm
    end: string // HH:mm
}

/** Initializes a new booking with the next available default time window. */
export function createEmptyForm(): BookingForm {
    const defaultWindow = getDefaultBookingWindow()

    return {
        subject: '',
        topic: '',
        roomId: 0,
        date: defaultWindow.date,
        start: defaultWindow.start,
        end: defaultWindow.end,
    }
}
/** Reads room and time selections from the room overview URL. */
export function createFormFromParams(params: URLSearchParams): BookingForm {
    const form = createEmptyForm()

    const date = params.get('date')
    const start = params.get('start')
    const end = params.get('end')
    const roomId = Number(params.get('roomId'))

    // Accept an entire valid time window or retain the default; do not mix dates and times.
    if (date && start && end && hasOrderedBookingTimes(date, start, end)) {
        form.date = date
        form.start = start
        form.end = end
    }

    if (Number.isInteger(roomId) && roomId > 0) {
        form.roomId = roomId
    }

    return form
}
