import { useState } from 'react'
import type { FormEvent } from 'react'
import { useAsyncData } from '../../hooks/useAsyncData'
import { errorText } from '../../utils/errorHandling'

export interface NamedEntity {
    id: number
    name: string
    detail: string | null
}

interface NamedEntityPageProps {
    title: string
    singular: string
    description: string
    detailLabel: string
    nameLimit: number
    namePlaceholder: string
    load: () => Promise<NamedEntity[]>
    create: (name: string, detail: string) => Promise<void>
    update: (id: number, name: string, detail: string) => Promise<void>
    remove: (id: number) => Promise<void>
}

const emptyForm = { name: '', detail: '' }

/** Shared CRUD interface for buildings and amenities, which have the same form shape. */
export default function NamedEntityPage({ title, singular, description, detailLabel,
    nameLimit, namePlaceholder, load, create, update, remove }: NamedEntityPageProps) {
    const [form, setForm] = useState(emptyForm)
    const [editingId, setEditingId] = useState<number | null>(null)
    const [saving, setSaving] = useState(false)
    const [error, setError] = useState('')
    const entity = singular.toLowerCase()

    const { data, loading, error: loadError, refresh } = useAsyncData(
        load, `Could not load ${title.toLowerCase()}.`
    )
    const items = data ?? []

    /** Clears the form without changing stored data. */
    function cancelEditing() {
        setEditingId(null)
        setForm(emptyForm)
    }

    /** Copies the selected entity into the editable form. */
    function startEditing(item: NamedEntity) {
        setEditingId(item.id)
        setForm({ name: item.name, detail: item.detail ?? '' })
        setError('')
    }

    /** Validates the input, saves the entity and reloads the list. */
    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault()
        if (saving || loading || loadError) return
        const name = form.name.trim()
        if (!name) {
            setError('Name is required.')
            return
        }

        setSaving(true)
        setError('')
        try {
            if (editingId === null) await create(name, form.detail.trim())
            else await update(editingId, name, form.detail.trim())
            cancelEditing()
            refresh()
        } catch (error) {
            setError(errorText(error, `Could not save ${entity}.`))
        } finally {
            setSaving(false)
        }
    }

    /** Deletes an entity only after confirmation; the API checks its relationships. */
    async function handleDelete(id: number) {
        if (saving || loading || loadError || !window.confirm(`Delete this ${entity}?`)) return
        setSaving(true)
        setError('')
        try {
            await remove(id)
            if (editingId === id) cancelEditing()
            refresh()
        } catch (error) {
            setError(errorText(error, `Could not delete ${entity}.`))
        } finally {
            setSaving(false)
        }
    }

    return (
        <main className="container py-5">
            <header className="mb-4">
                <span className="section-eyebrow">Facilities</span>
                <h1 className="h2 fw-bold mt-1 mb-2">{title}</h1>
                <p className="text-secondary-emphasis mb-0">{description}</p>
            </header>

            {error && <div className="error-message mb-4" role="alert">{error}</div>}
            {loading ? <p className="text-secondary">Loading {title.toLowerCase()}...</p> : loadError ? (
                <div className="error-message mb-4" role="alert">
                    {loadError}{' '}
                    <button type="button" className="btn btn-sm btn-light" onClick={refresh}>Try again</button>
                </div>
            ) : (
                <>
                    <div className="card mb-5">
                        <div className="card-body p-4">
                            <h2 className="h5 fw-bold mb-4">{editingId === null ? `Add ${singular}` : `Edit ${singular}`}</h2>
                            <form onSubmit={handleSubmit}>
                                <fieldset disabled={saving}>
                                    <div className="row g-3">
                                        <div className="col-12 col-md-5">
                                            <label className="form-label" htmlFor="entity-name">Name</label>
                                            <input id="entity-name" className="form-control" required maxLength={nameLimit}
                                                placeholder={namePlaceholder} value={form.name}
                                                onChange={event => setForm({ ...form, name: event.target.value })} />
                                        </div>
                                        <div className="col-12 col-md-7">
                                            <label className="form-label" htmlFor="entity-detail">{detailLabel}</label>
                                            <input id="entity-detail" className="form-control" maxLength={200}
                                                placeholder={`Optional ${detailLabel.toLowerCase()}`} value={form.detail}
                                                onChange={event => setForm({ ...form, detail: event.target.value })} />
                                        </div>
                                    </div>
                                    <div className="d-flex gap-2 justify-content-end mt-4">
                                        {editingId !== null && <button type="button" className="btn btn-light px-4" onClick={cancelEditing}>Cancel</button>}
                                        <button type="submit" className="btn btn-primary px-4">
                                            {saving ? 'Saving…' : editingId === null ? `Add ${singular}` : 'Save Changes'}
                                        </button>
                                    </div>
                                </fieldset>
                            </form>
                        </div>
                    </div>

                    {items.length === 0 ? <p className="text-secondary">No {title.toLowerCase()} registered yet.</p> : (
                        <div className="row g-4">
                            {items.map(item => (
                                <div className="col-12 col-md-6 col-lg-4" key={item.id}>
                                    <div className="card h-100">
                                        <div className="card-body p-4">
                                            <h2 className="h5 fw-bold">{item.name}</h2>
                                            <p className="text-secondary mb-0">{item.detail || `No ${detailLabel.toLowerCase()} registered.`}</p>
                                        </div>
                                        <div className="card-footer bg-transparent border-top-0 px-4 pb-4">
                                            <div className="d-flex gap-2">
                                                <button type="button" className="btn btn-outline-primary flex-grow-1" disabled={saving} onClick={() => startEditing(item)}>Edit</button>
                                                <button type="button" className="btn btn-outline-danger" disabled={saving} onClick={() => handleDelete(item.id)}>Delete</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </>
            )}
        </main>
    )
}
