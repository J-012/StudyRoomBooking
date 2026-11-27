import { useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'

import { login } from '../api/authApi'
import { useAuth } from '../context/useAuth'
import { errorText } from '../utils/errorHandling'

/** Login form for existing student and administrator accounts. */
function LoginPage() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')

    const [errors, setErrors] = useState<string[]>([])
    const [saving, setSaving] = useState(false)

    const { login: setAuth } = useAuth()
    const navigate = useNavigate()

    /** Collects client-side validation errors before sending credentials. */
    function validate(): string[] {
        const problems: string[] = []

        if (!/^\S+@\S+\.\S+$/.test(email)) {
            problems.push('Oppgi en gyldig e-postadresse.')
        }

        if (password.length === 0) {
            problems.push('Passord er påkrevd.')
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

            const auth = await login({ email, password })

            setAuth(auth)
            navigate('/')
        } catch (error) {
            setErrors([errorText(error, 'Feil e-post eller passord.')])
        } finally {
            setSaving(false)
        }
    }

    return (
        <main className="container py-5" style={{ maxWidth: 420 }}>
            <h1 className="h3 fw-bold mb-4">Logg inn</h1>

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

                <div className="mb-4">
                    <label className="form-label" htmlFor="password">Passord</label>
                    <input
                        id="password"
                        autoComplete="current-password"
                        type="password"
                        className="form-control"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                <button type="submit" className="btn btn-primary w-100" disabled={saving}>
                    {saving ? 'Vent…' : 'Logg inn'}
                </button>

                <p className="text-center mt-3 mb-0">
                    Har du ikke en bruker?{' '}
                    <Link to="/register">
                        Registrer deg
                    </Link>
                </p>
            </form>
        </main>
    )
}

export default LoginPage
