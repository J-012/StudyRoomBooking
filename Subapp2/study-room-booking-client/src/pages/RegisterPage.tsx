import { useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'

import { register } from '../api/authApi'
import { useAuth } from '../context/AuthContext'

function RegisterPage() {
    const [fullName, setFullName] = useState('')
    const [studentNumber, setStudentNumber] = useState('')
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [confirmPassword, setConfirmPassword] = useState('')

    const [errors, setErrors] = useState<string[]>([])

    const { login: setAuth } = useAuth()
    const navigate = useNavigate()

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

        if (!/^\S+@\S+\.\S+$/.test(email)) {
            problems.push('Oppgi en gyldig e-postadresse.')
        }

        if (password.length < 6) {
            problems.push('Passordet må være minst 6 tegn.')
        }

        if (password !== confirmPassword) {
            problems.push('Passordene er ikke like.')
        }

        return problems
    }

    async function handleSubmit(
        event: FormEvent<HTMLFormElement>
    ) {
        event.preventDefault()

        const validationErrors = validate()

        if (validationErrors.length > 0) {
            setErrors(validationErrors)
            return
        }

        try {
            setErrors([])

            const auth = await register({
                fullName,
                studentNumber,
                email,
                password,
            })

            setAuth(auth)
            navigate('/')
        } catch (err) {
            if (err instanceof Error) {
                setErrors([err.message])
            } else {
                setErrors(['Kunne ikke registrere bruker.'])
            }
        }
    }

    return (
        <main className="container py-5" style={{ maxWidth: 420 }}>
            <h1 className="h3 fw-bold mb-4">Registrer deg</h1>

            {errors.length > 0 && (
                <div className="error-message mb-4">
                    <ul className="mb-0 ps-3">
                        {errors.map((message, index) => (
                            <li key={index}>{message}</li>
                        ))}
                    </ul>
                </div>
            )}

            <form onSubmit={handleSubmit} noValidate>
                <div className="mb-3">
                    <label className="form-label">Fullt navn</label>
                    <input
                        className="form-control"
                        value={fullName}
                        maxLength={100}
                        onChange={(e) => setFullName(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label">Studentnummer</label>
                    <input
                        className="form-control"
                        value={studentNumber}
                        maxLength={20}
                        onChange={(e) => setStudentNumber(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label">E-post</label>
                    <input
                        type="email"
                        className="form-control"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>

                <div className="mb-3">
                    <label className="form-label">Passord</label>
                    <input
                        type="password"
                        className="form-control"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <div className="form-text">
                        Minst 6 tegn.
                    </div>
                </div>

                <div className="mb-4">
                    <label className="form-label">Bekreft passord</label>
                    <input
                        type="password"
                        className="form-control"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                    />
                </div>

                <button type="submit" className="btn btn-primary w-100">
                    Registrer
                </button>
            </form>
        </main>
    )
}

export default RegisterPage