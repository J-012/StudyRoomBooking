using Microsoft.AspNetCore.Identity;
using Moq;
using StudyRoomBooking.Models;
using StudyRoomBooking.Repositories;
using StudyRoomBooking.Services;
using Xunit;

namespace Subapp1.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_userRepo.Object);
    }

    private static User CreateUser(string userId, string password)
    {
        var user = new User
        {
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, password);

        return user;
    }

    // Positive tests

    [Fact]
    public async Task RegisterAsync_ValidUser_Succeeds()
    {
        _userRepo.Setup(r => r.ExistsAsync("student1"))
            .ReturnsAsync(false);

        var result = await _service.RegisterAsync(
            "student1",
            "Student One",
            "password123",
            "student1@example.com",
            "password123");

        Assert.True(result.Succeeded);

        _userRepo.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.UserId == "student1" &&
            u.Role == UserRole.Student &&
            !string.IsNullOrEmpty(u.PasswordHash) &&
            u.PasswordHash != "password123"
        )), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_CorrectPassword_ReturnsUser()
    {
        var user = CreateUser("student1", "password123");

        _userRepo.Setup(r => r.GetByUserIdAsync("student1"))
            .ReturnsAsync(user);

        var result = await _service.AuthenticateAsync(
            "student1", "password123");

        Assert.NotNull(result);
        Assert.Equal("student1", result.UserId);
    }

    // Negative tests

    [Fact]
    public async Task RegisterAsync_DifferentPasswords_Fails()
    {
        var result = await _service.RegisterAsync(
            "student1",
            "Student One",
            "password123",
            "student1@example.com",
            "wrongpassword");

        Assert.False(result.Succeeded);
        Assert.Equal("Passwords do not match.", result.ErrorMessage);

        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPassword_ReturnsNull()
    {
        var user = CreateUser("student1", "password123");

        _userRepo.Setup(r => r.GetByUserIdAsync("student1"))
            .ReturnsAsync(user);

        var result = await _service.AuthenticateAsync(
            "student1", "wrongpassword");

        Assert.Null(result);
    }
}