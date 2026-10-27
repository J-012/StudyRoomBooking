namespace StudyRoomBooking.Services;

public class JoinBookingResult
{
    public bool IsSuccess { get; private set; }

    public string? Error { get; private set; }

    public static JoinBookingResult Success()
    {
        return new JoinBookingResult
        {
            IsSuccess = true
        };
    }

    public static JoinBookingResult Failure(string error)
    {
        return new JoinBookingResult
        {
            IsSuccess = false,
            Error = error
        };
    }
}