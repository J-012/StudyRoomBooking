import type { Student, StudentRequest } from '../types/Student'

const studentsUrl = '/api/students'

export async function getStudents(): Promise<Student[]> {
    const response = await fetch(studentsUrl)

    if (!response.ok) {
        throw new Error('Could not load students')
    }

    const students: Student[] = await response.json()

    return students
}

export async function createStudent(
    student: StudentRequest
): Promise<Student> {
    const response = await fetch(studentsUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(student),
    })

    if (!response.ok) {
        throw new Error('Could not create student')
    }

    const createdStudent: Student = await response.json()

    return createdStudent
}

export async function updateStudent(
    id: number,
    student: StudentRequest
): Promise<void> {
    const response = await fetch(`${studentsUrl}/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(student),
    })

    if (!response.ok) {
        throw new Error('Could not update student')
    }
}

export async function deleteStudent(id: number): Promise<void> {
    const response = await fetch(`${studentsUrl}/${id}`, {
        method: 'DELETE',
    })

    if (!response.ok) {
        throw new Error('Could not delete student')
    }
}