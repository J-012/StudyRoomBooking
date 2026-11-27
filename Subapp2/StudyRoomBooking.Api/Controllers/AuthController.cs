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

    // New users are always students.
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

        IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            // email already taken or password too weak.
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        await _userManager.AddToRoleAsync(user, Roles.Student);

        _logger.LogInformation("New user registered: {Email}", user.Email);

        return Ok(await CreateAuthResponseAsync(user));
    }
    // LOGIN 
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            _logger.LogWarning("Login failed for {Email}", loginDto.Email);
            return Unauthorized("Invalid email or password.");
        }

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return Ok(await CreateAuthResponseAsync(user));
    }

    // Shared by register and login.
    private async Task<AuthResponseDto> CreateAuthResponseAsync(ApplicationUser user)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);

        return new AuthResponseDto
        {
            Token = _tokenService.CreateToken(user, roles),
            Email = user.Email!,
            FullName = user.FullName,
            Role = roles.FirstOrDefault() ?? Roles.Student
        };
    }
}
