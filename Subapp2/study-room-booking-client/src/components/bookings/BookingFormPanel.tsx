import type { FormEvent } from 'react'
import type { BookingForm } from '../../utils/bookingForm'
import type { Room } from '../../types/Room'
import { formatLocalDate } from '../../utils/dateFormatting'

interface BookingFormPanelProps {
    form: BookingForm
    isEditing: boolean
    roomsLoading: boolean
    timesValid: boolean
    selectedRoomId: number
    availabilityReady: boolean
    availabilityError: string
    refreshAvailability: () => void
    availableRooms: Room[]
    saving: boolean
    formError: string
    updateField: <K extends keyof BookingForm>(key: K, value: BookingForm[K]) => void
    handleSubmit: (event: FormEvent<HTMLFormElement>) => void
    cancelEditing: () => void
}

/** Presents the booking form; data loading and mutations remain in the page. */
export default function BookingFormPanel({ form, isEditing, roomsLoading, timesValid,
    selectedRoomId, availabilityReady, availabilityError, refreshAvailability, availableRooms, saving,
    formError, updateField, handleSubmit, cancelEditing }: BookingFormPanelProps) {
    return (
        <aside
            className={`card booking-panel ${isEditing ? 'is-editing' : ''}`}
        >
            <div className="card-body p-4">
                <div className="d-flex justify-content-between align-items-center mb-3">
                    <h2 className="h5 fw-bold mb-0">
                        {isEditing ? 'Endre booking' : 'Ny booking'}
                    </h2>
                    {isEditing && (
                        <button
                            type="button"
                            className="btn btn-sm btn-link text-secondary text-decoration-none p-0"
                            disabled={saving}
                            onClick={cancelEditing}
                        >
                            Avbryt
                        </button>
                    )}
                </div>

                <form
                    onSubmit={handleSubmit}
                    className="d-flex flex-column gap-3"
                >
                    <fieldset disabled={saving} className="d-flex flex-column gap-3">
                        <div>
                            <label className="form-label" htmlFor="date">
                                Dato
                            </label>
                            <input
                                id="date"
                                type="date"
                                className="form-control"
                                required
                                min={isEditing ? undefined : formatLocalDate()}
                                value={form.date}
                                onChange={(e) =>
                                    updateField('date', e.target.value)
                                }
                            />
                        </div>

                        <div className="row g-2">
                            <div className="col-6">
                                <label className="form-label" htmlFor="start">
                                    Fra
                                </label>
                                <input
                                    id="start"
                                    type="time"
                                    className="form-control"
                                    required
                                    step={900}
                                    value={form.start}
                                    onChange={(e) =>
                                        updateField('start', e.target.value)
                                    }
                                />
                            </div>
                            <div className="col-6">
                                <label className="form-label" htmlFor="end">
                                    Til
                                </label>
                                <input
                                    id="end"
                                    type="time"
                                    className="form-control"
                                    required
                                    step={900}
                                    value={form.end}
                                    onChange={(e) =>
                                        updateField('end', e.target.value)
                                    }
                                />
                            </div>
                        </div>

                        {form.start !== '' &&
                            form.end !== '' &&
                            form.end <= form.start && (
                                <div className="form-hint text-danger">
                                    Sluttid må være etter starttid.
                                </div>
                            )}

                        <div>
                            <span className="form-label d-block">
                                Ledige rom
                                {availabilityReady && (
                                    <span className="text-secondary fw-normal">
                                        {' '}({availableRooms.length})
                                    </span>
                                )}
                            </span>

                            {!timesValid ? (
                                <div className="room-placeholder">
                                    Velg dato og tid for å se ledige rom.
                                </div>
                            ) : roomsLoading ? (
                                <div className="room-placeholder">
                                    Henter ledige rom…
                                </div>
                            ) : availabilityError ? (
                                <div className="room-placeholder" role="alert">
                                    Ledighet kunne ikke sjekkes.{' '}
                                    <button type="button" className="btn btn-sm btn-light" onClick={refreshAvailability}>Prøv igjen</button>
                                </div>
                            ) : availableRooms.length === 0 ? (
                                <div className="room-placeholder">
                                    Ingen ledige rom i dette tidsrommet.
                                    Prøv et annet tidspunkt.
                                </div>
                            ) : (
                                <div
                                    className="room-options"
                                    role="radiogroup"
                                    aria-label="Ledige rom"
                                >
                                    {availableRooms.map((room) => {
                                        const selected =
                                            selectedRoomId === room.id
                                        return (
                                            <button
                                                key={room.id}
                                                type="button"
                                                role="radio"
                                                aria-checked={selected}
                                                className={`room-option ${selected ? 'is-selected' : ''}`}
                                                onClick={() =>
                                                    updateField(
                                                        'roomId',
                                                        room.id
                                                    )
                                                }
                                            >
                                                <span className="room-option-number">
                                                    {room.roomNumber}
                                                </span>
                                                <span className="room-option-building">
                                                    {room.building}
                                                </span>
                                            </button>
                                        )
                                    })}
                                </div>
                            )}
                        </div>

                        <hr className="my-1" />

                        <div>
                            <label className="form-label" htmlFor="subject">
                                Fag
                            </label>
                            <input
                                id="subject"
                                className="form-control"
                                placeholder="F.eks. DATA2410"
                                required
                                maxLength={100}
                                value={form.subject}
                                onChange={(e) =>
                                    updateField('subject', e.target.value)
                                }
                            />
                        </div>

                        <div>
                            <label className="form-label" htmlFor="topic">
                                Tema
                            </label>
                            <input
                                id="topic"
                                className="form-control"
                                placeholder="Hva skal dere jobbe med?"
                                required
                                maxLength={200}
                                value={form.topic}
                                onChange={(e) =>
                                    updateField('topic', e.target.value)
                                }
                            />
                        </div>

                        {(formError || availabilityError) && (
                            <div className="form-error" role="alert">{formError || availabilityError}</div>
                        )}

                        <button
                            type="submit"
                            className="btn btn-primary w-100 py-2"
                            disabled={saving || roomsLoading || !availabilityReady || selectedRoomId <= 0 || !timesValid}
                        >
                            {saving
                                ? 'Lagrer…'
                                : isEditing
                                    ? 'Lagre endringer'
                                    : 'Book rom'}
                        </button>

                    </fieldset>
                </form>
            </div>
        </aside>
    )
}
