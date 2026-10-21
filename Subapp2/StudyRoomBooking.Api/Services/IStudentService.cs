using StudyRoomBooking.Api.DTOs;

namespace StudyRoomBooking.Api.Services;

public interface IStudentService
{
    Task<List<StudentDto>> GetAllAsync();
    Task<StudentDto?> GetByIdAsync(int id);
    Task<StudentDto> CreateAsync(CreateStudentDto studentDto);
    Task<bool> UpdateAsync(int id, UpdateStudentDto studentDto);
    Task<bool> DeleteAsync(int id);
}