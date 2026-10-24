namespace StudyRoomBooking.Services;

public sealed class UserRegistrationResult
{
    private UserRegistrationResult(bool succeeded, string? errorMessage)
    {
        Succeeded = succeeded;
        ErrorMessage = errorMessage;
    }

    public bool Succeeded { get; }

    public string? ErrorMessage { get; }

    public static UserRegistrationResult Success() => new(true, null);

    public static UserRegistrationResult Failure(string errorMessage) => new(false, errorMessage);
}
