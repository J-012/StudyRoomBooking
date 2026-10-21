using StudyRoomBooking.Api.Models;

namespace StudyRoomBooking.Api.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student> AddAsync(Student student);
    Task DeleteAsync(Student student);
    Task SaveChangesAsync();
}