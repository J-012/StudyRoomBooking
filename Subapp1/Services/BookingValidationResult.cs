namespace StudyRoomBooking.Services;

    // Representerer resultatet av validering av en booking
public sealed class BookingValidationResult
{
    private readonly List<ValidationError> _errors = new();

    public bool IsValid => _errors.Count == 0;

    public IReadOnlyList<ValidationError> Errors => _errors;

    public void AddError(string key, string message)
    {
        _errors.Add(new ValidationError(key, message));
    }
}
