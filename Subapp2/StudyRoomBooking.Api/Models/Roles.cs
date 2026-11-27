namespace StudyRoomBooking.Api.Models;

// The two roles in the app. Constants so they can be used in
public static class Roles
{
    public const string Admin = "Admin";
    public const string Student = "Student";

    public static readonly string[] All = { Admin, Student };
}
