namespace StudyRoomBooking.Models

{
    public class Student
    {
        public string StudentId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        
        public string PasswordHash { get; set; } = "";    

        public string Email { get; set; } = string.Empty;
    }
}