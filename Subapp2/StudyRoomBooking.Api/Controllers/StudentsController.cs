using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;
    // Kontruktør 
    public StudentsController(
        IStudentService studentService,
        ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    // GET /api/students
    [HttpGet]
    public async Task<ActionResult<List<StudentDto>>> GetAll()
    {
        List<StudentDto> students = await _studentService.GetAllAsync();

        return Ok(students);
    }
    // GET /api/students baseret på id
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        StudentDto? student = await _studentService.GetByIdAsync(id);

        if (student == null)
        {
            _logger.LogWarning("Student with id {StudentId} was not found.", id);
            return NotFound();
        }

        return Ok(student);
    }

    // POST /api/students lager ny student
    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(CreateStudentDto studentDto)
    {
        StudentDto createdStudent = await _studentService.CreateAsync(studentDto);

        //Logger information om at en student er blevet oprettet
        _logger.LogInformation("Student {StudentId} was created.", createdStudent.Id);

        // Returnerer HTTP 201 Created.
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdStudent.Id },
            createdStudent
        );
    }
    // PUT /api/students baseret på id; opdaterer en student
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateStudentDto studentDto)
    {
        bool updated = await _studentService.UpdateAsync(id, studentDto);

        //Hvis opdateringen ikke ble velykket.
        if (!updated)
        {
            _logger.LogWarning("Student with id {StudentId} could not be updated because it was not found.", id);
            return NotFound();
        }
        //Hvis opdateringen var velykket.
        _logger.LogInformation("Student {StudentId} was updated.", id);

        return NoContent();
    }

    // DELETE /api/students baseret på id.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _studentService.DeleteAsync(id);
        //Hvis sletningen ikke ble velykket.
        if (!deleted)
        {
            _logger.LogWarning("Student with id {StudentId} could not be deleted because it was not found.", id);
            return NotFound();
        }
        //Hvis sletningen var velykket.
        _logger.LogInformation("Student {StudentId} was deleted.", id);

        return NoContent();
    }
}