export interface AuthResponse {
    token: string
    email: string
    fullName: string
    role: string
}

export interface LoginRequest {
    email: string
    password: string
}

export interface RegisterRequest {
    fullName: string
    studentNumber: string
    email: string
    password: string
}