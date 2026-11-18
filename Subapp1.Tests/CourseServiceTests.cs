using Moq;
using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;
using StudyRoomBooking.Services;
using Xunit;

namespace Subapp1.Tests;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _courseRepo = new();
    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _service = new CourseService(_courseRepo.Object);
    }

    // Positive test

    [Fact]
    public async Task CreateAsync_ValidCourse_IsSaved()
    {
        var course = new Course
        {
            CourseCode = "  itpe3200 ",
            Name = " Web Applications ",
            Description = ""
        };

        _courseRepo.Setup(r => r.CourseCodeExistsAsync("ITPE3200"))
            .ReturnsAsync(false);

        var error = await _service.CreateAsync(course);

        Assert.Null(error);

        _courseRepo.Verify(r => r.AddAsync(It.Is<Course>(c =>
            c.CourseCode == "ITPE3200" &&
            c.Name == "Web Applications"
        )), Times.Once);
    }

    // Negative test

    [Fact]
    public async Task CreateAsync_DuplicateCourseCode_IsRejected()
    {
        var course = new Course
        {
            CourseCode = "ITPE3200",
            Name = "Web Applications"
        };

        _courseRepo.Setup(r => r.CourseCodeExistsAsync("ITPE3200"))
            .ReturnsAsync(true);

        var error = await _service.CreateAsync(course);

        Assert.Equal("A course with this code already exists.", error);

        _courseRepo.Verify(r => r.AddAsync(It.IsAny<Course>()),
            Times.Never);
    }
}