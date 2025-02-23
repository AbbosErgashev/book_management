using BookManagement.DataAccess.IServices;
using BookManagement.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Endpoint for user registration.
    /// </summary>
    /// <param name="userDto">User data for registration.</param>
    /// <returns>Action result indicating registration status.</returns>
    [HttpPost("signup")]
    public async Task<IActionResult> RegisterUserAsync([FromBody] UserDto userDto)
    {
        _logger.LogInformation("Attempting to register user with email: {Email}", userDto.Email);

        var isRegistered = await _userService.RegisterUserAsync(userDto);

        if (!isRegistered)
        {
            _logger.LogWarning("User with email {Email} is already registered.", userDto.Email);
            return BadRequest("User is already registered.");
        }

        _logger.LogInformation("User with email {Email} successfully registered.", userDto.Email);
        return Ok("User successfully registered.");
    }

    /// <summary>
    /// Endpoint for user authentication.
    /// </summary>
    /// <param name="userDto">User data for authentication.</param>
    /// <returns>Action result containing JWT token upon successful authentication.</returns>
    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync([FromBody] UserDto userDto)
    {
        _logger.LogInformation("Attempting to sign in user with email: {Email}", userDto.Email);

        var token = await _userService.AuthenticateUserAsync(userDto.Email, userDto.Password);

        if (token == null)
        {
            _logger.LogWarning("Failed sign-in attempt for email: {Email}. Incorrect email or password.", userDto.Email);
            return Unauthorized("Incorrect email or password.");
        }

        _logger.LogInformation("User with email {Email} successfully signed in.", userDto.Email);
        return Ok(new { Token = token });
    }
}
