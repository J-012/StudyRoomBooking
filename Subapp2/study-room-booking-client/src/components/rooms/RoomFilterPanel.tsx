import type { Building } from '../../types/Building'

export interface RoomFilters {
    search: string
    buildingId: number
    minCapacity: number
    date: string
    start: string
    end: string
    onlyAvailable: boolean
}

interface RoomFilterPanelProps {
    filters: RoomFilters
    buildings: Building[]
    minDate: string
    timesValid: boolean
    availabilityReady: boolean
    updateFilter: <K extends keyof RoomFilters>(key: K, value: RoomFilters[K]) => void
}

const capacityOptions = [0, 2, 4, 6, 8, 10]

/** Presents the controlled search and availability filters for the room overview. */
export default function RoomFilterPanel({ filters, buildings, minDate, timesValid, availabilityReady, updateFilter }: RoomFilterPanelProps) {
    const { search, buildingId, minCapacity, date, start, end, onlyAvailable } = filters

    return (
        <section className="card filter-panel sticky-panel" aria-label="Filtre">
            <div className="card-body p-3 p-md-4">
                <div className="row g-3">
                    <div className="col-12">
                        <label
                            className="form-label"
                            htmlFor="room-search"
                        >
                            Søk
                        </label>
                        <input
                            id="room-search"
                            type="search"
                            className="form-control"
                            placeholder="Romnummer eller bygg"
                            value={search}
                            onChange={(e) =>
                                updateFilter('search', e.target.value)
                            }
                        />
                    </div>

                    <div className="col-6 col-lg-12">
                        <label
                            className="form-label"
                            htmlFor="room-building"
                        >
                            Bygg
                        </label>

                        <select
                            id="room-building"
                            className="form-select"
                            value={buildingId}
                            onChange={(event) =>
                                updateFilter('buildingId', Number(event.target.value))
                            }
                        >
                            <option value={0}>
                                Alle bygg
                            </option>

                            {buildings.map(
                                (building) => (
                                    <option
                                        key={building.id}
                                        value={building.id}
                                    >
                                        {building.name}
                                    </option>
                                )
                            )}
                        </select>
                    </div>

                    <div className="col-6 col-lg-12">
                        <label
                            className="form-label"
                            htmlFor="room-capacity"
                        >
                            Plasser
                        </label>
                        <select
                            id="room-capacity"
                            className="form-select"
                            value={minCapacity}
                            onChange={(e) =>
                                updateFilter('minCapacity', Number(e.target.value))
                            }
                        >
                            {capacityOptions.map((value) => (
                                <option key={value} value={value}>
                                    {value === 0
                                        ? 'Alle størrelser'
                                        : `Minst ${value}`}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                <hr className="my-3" />

                <span className="filter-heading">Tidsrom</span>

                <div className="row g-3 align-items-end">
                    <div className="col-12 col-md-4 col-lg-12">
                        <label
                            className="form-label"
                            htmlFor="room-date"
                        >
                            Dato
                        </label>
                        <input
                            id="room-date"
                            type="date"
                            className="form-control"
                            min={minDate}
                            value={date}
                            onChange={(e) =>
                                updateFilter('date', e.target.value)
                            }
                        />
                    </div>

                    <div className="col-6 col-md-4 col-lg-6">
                        <label
                            className="form-label"
                            htmlFor="room-start"
                        >
                            Fra
                        </label>
                        <input
                            id="room-start"
                            type="time"
                            step={900}
                            className="form-control"
                            value={start}
                            onChange={(e) =>
                                updateFilter('start', e.target.value)
                            }
                        />
                    </div>

                    <div className="col-6 col-md-4 col-lg-6">
                        <label
                            className="form-label"
                            htmlFor="room-end"
                        >
                            Til
                        </label>
                        <input
                            id="room-end"
                            type="time"
                            step={900}
                            className="form-control"
                            value={end}
                            onChange={(e) => updateFilter('end', e.target.value)}
                        />
                    </div>

                    <div className="col-12">
                        <div className="form-check form-switch filter-switch">
                            <input
                                id="only-available"
                                type="checkbox"
                                role="switch"
                                className="form-check-input"
                                checked={onlyAvailable}
                                disabled={!onlyAvailable && (!timesValid || !availabilityReady)}
                                onChange={(e) =>
                                    updateFilter('onlyAvailable', e.target.checked)
                                }
                            />
                            <label
                                className="form-check-label"
                                htmlFor="only-available"
                            >
                                Vis bare ledige rom
                            </label>
                        </div>
                    </div>
                </div>

                {start !== '' && end !== '' && end <= start && (
                    <p className="text-danger small mt-2 mb-0">
                        Sluttid må være etter starttid.
                    </p>
                )}
            </div>
        </section>
    )
}
