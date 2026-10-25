import { useState } from 'react'
import type { FormEvent } from 'react'
import { useNavigate } from 'react-router-dom'

import { login } from '../api/authApi'
import { useAuth } from '../context/AuthContext'

function LoginPage() {
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')

    const [errors, setErrors] = useState<string[]>([])

    const { login: setAuth } = useAuth()
    const navigate = useNavigate()

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

            const auth = await login({ email, password })

            setAuth(auth)
            navigate('/')
        } catch {
            setErrors(['Feil e-post eller passord.'])
        }
    }

    return (
        <main className="container py-5" style={{ maxWidth: 420 }}>
            <h1 className="h3 fw-bold mb-4">Logg inn</h1>

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
                    <label className="form-label">E-post</label>
                    <input
                        type="email"
                        className="form-control"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>

                <div className="mb-4">
                    <label className="form-label">Passord</label>
                    <input
                        type="password"
                        className="form-control"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>

                <button type="submit" className="btn btn-primary w-100">
                    Logg inn
                </button>
            </form>
        </main>
    )
}

export default LoginPage