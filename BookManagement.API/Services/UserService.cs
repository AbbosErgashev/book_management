using BookManagement.DataAccess;
using BookManagement.DataAccess.IServices;
using BookManagement.Models;
using BookManagement.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookManagement.API.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _appContext;
    private readonly IConfiguration _config;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, IConfiguration configuration, ILogger<UserService> logger)
    {
        _appContext = context;
        _config = configuration;
        _logger = logger;
    }

    public async Task<bool> RegisterUserAsync(UserDto userDto)
    {
        var existingUser = await _appContext.Users
            .FirstOrDefaultAsync(u => u.Email == userDto.Email);

        if (existingUser != null)
        {
            _logger.LogWarning("User with email '{Email}' already exists.", userDto.Email);
            return false;
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = userDto.Email,
            Password = userDto.Password
        };

        await _appContext.Users.AddAsync(newUser);
        await _appContext.SaveChangesAsync();
        _logger.LogInformation("User with email '{Email}' successfully registered.", userDto.Email);

        return true;
    }

    public async Task<string> AuthenticateUserAsync(string email, string password)
    {
        var adminEmail = _config["AdminSettings:Email"];
        var adminPassword = _config["AdminSettings:Password"];

        if (email == adminEmail && password == adminPassword)
        {
            _logger.LogInformation("Admin user with email '{Email}' authenticated successfully.", email);
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Email = adminEmail,
                Password = adminPassword
            };
            return GenerateJwtToken(adminUser);
        }

        var user = await _appContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || user.Password != password)
        {
            _logger.LogWarning("Failed authentication attempt for email '{Email}'.", email);
            return null;
        }

        _logger.LogInformation("User with email '{Email}' authenticated successfully.", email);
        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var secretKey = _config["TokenSetting:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["TokenSetting:Issuer"],
            audience: _config["TokenSetting:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        _logger.LogInformation("JWT token generated for user with email '{Email}'.", user.Email);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

