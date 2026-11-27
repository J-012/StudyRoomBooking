/** Authentication response containing a JWT and display information. */
export interface AuthResponse {
    token: string
    email: string
    fullName: string
    role: string
}

/** Credentials sent to the login endpoint. */
export interface LoginRequest {
    email: string
    password: string
}

/** Information required to register a student account. */
export interface RegisterRequest {
    fullName: string
    studentNumber: string
    email: string
    password: string
}
