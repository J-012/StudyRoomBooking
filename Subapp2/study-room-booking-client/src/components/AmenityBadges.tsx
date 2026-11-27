import type { Amenity } from '../types/Amenity'

/** Displays the facilities associated with a room in either room view. */
export default function AmenityBadges({ amenities, className = '' }: { amenities: Amenity[], className?: string }) {
    if (amenities.length === 0) return null
    return (
        <div className={`d-flex flex-wrap gap-1 ${className}`}>
            {amenities.map(amenity => <span key={amenity.id} className="badge text-bg-light">{amenity.name}</span>)}
        </div>
    )
}
