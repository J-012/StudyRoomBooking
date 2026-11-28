/** Checks the basic email shape before sending credentials to the API. */
export function isValidEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim())
}

/** Mirrors the current ASP.NET Identity password rules for registration. */
export function validateRegistrationPassword(password: string): string[] {
    const errors: string[] = []
    if (password.length < 6) errors.push('Passordet må være minst 6 tegn.')
    if (!/[a-z]/.test(password)) errors.push('Passordet må inneholde minst én liten bokstav (a–z).')
    if (!/[0-9]/.test(password)) errors.push('Passordet må inneholde minst ett tall (0–9).')
    return errors
}
