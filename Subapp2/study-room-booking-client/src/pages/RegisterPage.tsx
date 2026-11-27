import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'

import { register } from '../api/authApi'
import { useAuth } from '../context/useAuth'

import { isValidEmail, validateRegistrationPassword } from '../utils/authValidation'
import { errorText } from '../utils/errorHandling'

/** Registration form for new student accounts. */
function RegisterPage() {
    const [fullName, setFullName] = useState('')
    const [studentNumber, setStudentNumber] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [confirmPassword, setConfirmPassword] = useState('')

    const [errors, setErrors] = useState<string[]>([])
    const [saving, setSaving] = useState(false)

    const { login: setAuth } = useAuth()
    const navigate = useNavigate()

    /** Collects client-side validation errors before sending credentials. */
    function validate(): string[] {
        const problems: string[] = []

        if (fullName.trim().length === 0) {
            problems.push('Fullt navn er påkrevd.')
        } else if (fullName.length > 100) {
            problems.push('Fullt navn kan være maks 100 tegn.')
        }

        if (studentNumber.trim().length === 0) {
            problems.push('Studentnummer er påkrevd.')
        } else if (studentNumber.length > 20) {
            problems.push('Studentnummer kan være maks 20 tegn.')
        }

        if (!isValidEmail(email)) {
            problems.push('Oppgi en gyldig e-postadresse.')
        }

        problems.push(...validateRegistrationPassword(password))

        if (password !== confirmPassword) {
            problems.push('Passordene er ikke like.')
        }

        return problems
    }

    /** Submits the form and initializes the authenticated session on success. */
    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {
        event.preventDefault()
        if (saving) return

        const validationErrors = validate()

        if (validationErrors.length > 0) {
            setErrors(validationErrors)
            return
        }

        setSaving(true)
        try {
            setErrors([])

            const auth = await register({
                fullName: fullName.trim(),
                studentNumber: studentNumber.trim(),
                email: email.trim(),
                password,
            })

            setAuth(auth)
            navigate('/')
        } catch (error) {
            setErrors([errorText(error, 'Kunne ikke registrere bruker.')])
        } finally {
            setSaving(false)
        }
    }

    return (
        <main className="container py-5" style={{ maxWidth: 420 }}>
            <h1 className="h3 fw-bold mb-4">Registrer deg</h1>

            {errors.length > 0 && (
                <div className="error-message mb-4" role="alert">
                    <ul className="mb-0 ps-3">
                        {errors.map((message, index) => (
                            <li key={index}>{message}</li>
                        ))}
                    </ul>
                </div>
            )}

            <form onSubmit={handleSubmit} noValidate>
                <div className="mb-3">
                    <label className="form-label" htmlFor="full-name">Fullt navn</label>
                    <input
                        id="full-name"
                        autoComplete="name"
                        className="form-control"
                        value={fullName}
                        maxLength={100}
                        onChange={(e) => setFullName(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label" htmlFor="student-number">Studentnummer</label>
                    <input
                        id="student-number"
                        autoComplete="off"
                        className="form-control"
                        value={studentNumber}
                        maxLength={20}
                        onChange={(e) => setStudentNumber(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label" htmlFor="email">E-post</label>
                    <input
                        id="email"
                        autoComplete="email"
                        type="email"
                        className="form-control"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label" htmlFor="password">Passord</label>
                    <input
                        id="password"
                        autoComplete="new-password"
                        type="password"
                        className="form-control"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <div className="form-text">
                        Minst 6 tegn, én liten bokstav (a–z) og ett tall (0–9).
                    </div>
                </div>

                <div className="mb-4">
                    <label className="form-label" htmlFor="confirm-password">Bekreft passord</label>
                    <input
                        id="confirm-password"
                        autoComplete="new-password"
                        type="password"
                        className="form-control"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                    />
                </div>

                <button type="submit" className="btn btn-primary w-100" disabled={saving}>
                    {saving ? 'Vent…' : 'Registrer'}
                </button>

                <p className="text-center mt-3 mb-0">
                    Har du allerede en bruker?{' '}
                    <Link to="/login">
                        Logg inn
                    </Link>
                </p>
            </form>
        </main>
    )
}

export default RegisterPage
