using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.Api.DTOs;
using StudyRoomBooking.Api.Models;
using StudyRoomBooking.Api.Services;

namespace StudyRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            StudentNumber = registerDto.StudentNumber
        };

        IdentityResult result =
            await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        await _userManager.AddToRoleAsync(user, "Student");

        _logger.LogInformation("New user registered: {Email}", user.Email);

        string token = await _tokenService.CreateTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            Role = "Student"
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        ApplicationUser? user =
            await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            _logger.LogWarning("Login failed: unknown email {Email}", loginDto.Email);
            return Unauthorized("Invalid email or password.");
        }

        bool passwordValid =
            await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!passwordValid)
        {
            _logger.LogWarning("Login failed: wrong password for {Email}", loginDto.Email);
            return Unauthorized("Invalid email or password.");
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault() ?? "Student";

        string token = await _tokenService.CreateTokenAsync(user);

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            FullName = user.FullName,
            Role = role
        });
    }
}