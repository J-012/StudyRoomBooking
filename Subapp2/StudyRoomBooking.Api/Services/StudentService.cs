using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Repositories;

namespace StudyRoomBooking.Api.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }
    //Alle funksjonene som ikke er en bool returnerer i DTO format. 
    // Henter studenter og konverterer dem til DTO-er retunerer en liste.
    public async Task<List<StudentDto>> GetAllAsync()
    {
        List<Student> students = await _studentRepository.GetAllAsync();

        List<StudentDto> studentDtos = new List<StudentDto>();

        foreach (Student student in students)
        {
            studentDtos.Add(ToDto(student));
        }

        return studentDtos;
    }
    // Henter en student baseret på id. 
    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        Student? student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return null;
        }

        return ToDto(student);
    }
    // Opretter en ny student baseret på CreateStudentDto.
    public async Task<StudentDto> CreateAsync(CreateStudentDto studentDto)
    {
        Student student = new Student
        {
            FullName = studentDto.FullName,
            Email = studentDto.Email,
            StudentNumber = studentDto.StudentNumber
        };

        Student createdStudent = await _studentRepository.AddAsync(student);

        return ToDto(createdStudent);
    }

    // Oppdaterer en eksisterende student.
    public async Task<bool> UpdateAsync(int id, UpdateStudentDto studentDto)
    {
        Student? student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return false;
        }

        student.FullName = studentDto.FullName;
        student.Email = studentDto.Email;
        student.StudentNumber = studentDto.StudentNumber;

        await _studentRepository.SaveChangesAsync();

        return true;
    }

    // Sletter en student basert på ID.
    public async Task<bool> DeleteAsync(int id)
    {
        Student? student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return false;
        }

        await _studentRepository.DeleteAsync(student);

        return true;
    }
    // Konverterer en Student-model til en StudentDto.
    private StudentDto ToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            FullName = student.FullName,
            Email = student.Email,
            StudentNumber = student.StudentNumber
        };
    }
}