/** Returns the message from a caught Error, or a fallback for unknown values. */
export function errorText(error: unknown, fallback: string): string {
    return error instanceof Error ? error.message : fallback
}
