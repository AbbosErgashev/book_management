using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookManagement.API.Configurations;

public class JwtAuthenticationConfig
{
    private readonly ILogger<JwtAuthenticationConfig> _logger;
    private readonly IConfiguration _configuration;

    public JwtAuthenticationConfig(ILogger<JwtAuthenticationConfig> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void ConfigureJwtAuthentication(IServiceCollection services)
    {
        try
        {
            _logger.LogInformation("Configuring JWT Authentication...");

            var secretKey = _configuration["TokenSetting:SecretKey"];
            var issuer = _configuration["TokenSetting:Issuer"];
            var audience = _configuration["TokenSetting:Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                _logger.LogWarning("Missing JWT configuration settings: SecretKey, Issuer, or Audience.");
            }
            else
            {
                _logger.LogInformation("JWT configuration values loaded successfully.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = creds.Key
                    };
                });

            _logger.LogInformation("JWT Authentication configured successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while configuring JWT Authentication.");
            throw;
        }
    }
}
