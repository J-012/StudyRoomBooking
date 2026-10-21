import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'

import {
    createStudent,
    deleteStudent,
    getStudents,
    updateStudent,
} from '../api/studentsApi'

import type {
    Student,
    StudentRequest,
} from '../types/Student'

const emptyForm: StudentRequest = {
    fullName: '',
    email: '',
    studentNumber: '',
}

function StudentsPage() {
    const [students, setStudents] = useState<Student[]>([])
    const [form, setForm] = useState<StudentRequest>(emptyForm)

    const [editingId, setEditingId] =
        useState<number | null>(null)

    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    async function loadStudents() {
        try {
            setError('')

            const studentData = await getStudents()

            setStudents(studentData)
        } catch {
            setError('Could not load students.')
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadStudents()
    }, [])

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {
        event.preventDefault()

        try {
            setError('')

            if (editingId === null) {
                await createStudent(form)
            } else {
                await updateStudent(editingId, form)
            }

            setForm(emptyForm)
            setEditingId(null)

            await loadStudents()
        } catch {
            setError('Could not save student.')
        }
    }

    function startEditing(student: Student) {
        setEditingId(student.id)

        setForm({
            fullName: student.fullName,
            email: student.email,
            studentNumber: student.studentNumber,
        })
    }

    function cancelEditing() {
        setEditingId(null)
        setForm(emptyForm)
    }

    async function handleDelete(id: number) {
        const shouldDelete =
            window.confirm('Delete this student?')

        if (!shouldDelete) {
            return
        }

        try {
            await deleteStudent(id)
            await loadStudents()
        } catch {
            setError('Could not delete student.')
        }
    }

    if (loading) {
        return (
            <main className="container py-5">
                <p className="text-secondary">
                    Loading students...
                </p>
            </main>
        )
    }

    return (
        <main className="container py-5">

            <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-end gap-3 mb-4">

                <div>
                    <span className="section-eyebrow">
                        People
                    </span>

                    <h1 className="h2 fw-bold mt-1 mb-2">
                        Students
                    </h1>

                    <p className="text-secondary-emphasis mb-0">
                        {students.length > 0
                            ? `${students.length} ${
                                  students.length === 1
                                      ? 'student'
                                      : 'students'
                              } registered.`
                            : 'No students registered yet.'}
                    </p>
                </div>

            </div>

            {error && (
                <div className="error-message mb-4">
                    {error}
                </div>
            )}

            <div className="card mb-5">
                <div className="card-body p-4">

                    <span className="section-eyebrow">
                        Student management
                    </span>

                    <h2 className="h5 fw-bold mt-1 mb-4">
                        {editingId === null
                            ? 'Add Student'
                            : 'Edit Student'}
                    </h2>

                    <form onSubmit={handleSubmit}>

                        <div className="row g-3">

                            <div className="col-12 col-md-4">

                                <label className="form-label">
                                    Full name
                                </label>

                                <input
                                    className="form-control"
                                    value={form.fullName}
                                    required
                                    maxLength={100}
                                    placeholder="e.g. Ola Nordmann"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            fullName:
                                                event.target.value,
                                        })
                                    }
                                />

                            </div>

                            <div className="col-12 col-md-4">

                                <label className="form-label">
                                    Email
                                </label>

                                <input
                                    className="form-control"
                                    type="email"
                                    value={form.email}
                                    required
                                    maxLength={100}
                                    placeholder="e.g. ola@example.com"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            email:
                                                event.target.value,
                                        })
                                    }
                                />

                            </div>

                            <div className="col-12 col-md-4">

                                <label className="form-label">
                                    Student number
                                </label>

                                <input
                                    className="form-control"
                                    value={form.studentNumber}
                                    required
                                    maxLength={20}
                                    placeholder="e.g. S12345"
                                    onChange={(event) =>
                                        setForm({
                                            ...form,
                                            studentNumber:
                                                event.target.value,
                                        })
                                    }
                                />

                            </div>

                        </div>

                        <div className="d-flex gap-2 justify-content-end mt-4">

                            {editingId !== null && (
                                <button
                                    type="button"
                                    className="btn btn-light px-4"
                                    onClick={cancelEditing}
                                >
                                    Cancel
                                </button>
                            )}

                            <button
                                type="submit"
                                className="btn btn-primary px-4"
                            >
                                {editingId === null
                                    ? 'Add Student'
                                    : 'Save Changes'}
                            </button>

                        </div>

                    </form>

                </div>
            </div>

            {students.length > 0 ? (

                <div className="row g-4">

                    {students.map((student) => (

                        <div
                            className="col-12 col-sm-6 col-lg-4"
                            key={student.id}
                        >

                            <div className="card card-hover h-100">

                                <div className="card-body p-4 d-flex flex-column">

                                    <h2 className="h5 fw-bold mb-1">
                                        {student.fullName}
                                    </h2>

                                    <p className="text-secondary-emphasis mb-1">
                                        {student.email}
                                    </p>

                                    <p className="text-secondary-emphasis mb-0">
                                        {student.studentNumber}
                                    </p>

                                </div>

                                <div className="card-footer bg-transparent border-top-0 px-4 pb-4">

                                    <div className="d-flex gap-2">

                                        <button
                                            type="button"
                                            className="btn btn-outline-primary flex-grow-1"
                                            onClick={() =>
                                                startEditing(student)
                                            }
                                        >
                                            Edit
                                        </button>

                                        <button
                                            type="button"
                                            className="btn btn-outline-danger"
                                            onClick={() =>
                                                handleDelete(student.id)
                                            }
                                        >
                                            Delete
                                        </button>

                                    </div>

                                </div>

                            </div>

                        </div>

                    ))}

                </div>

            ) : (

                <div className="card">

                    <div className="card-body text-center py-5">

                        <h2 className="h5 fw-bold">
                            No students registered
                        </h2>

                        <p className="text-secondary-emphasis mb-0">
                            There are no students registered yet.
                        </p>

                    </div>

                </div>

            )}

        </main>
    )
}

export default StudentsPage