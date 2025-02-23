using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BookManagement.API.Configurations;

public class SwaggerConfig
{
    private readonly ILogger<SwaggerConfig> _logger;

    public SwaggerConfig(ILogger<SwaggerConfig> logger)
    {
        _logger = logger;
    }

    public void ConfigureSwagger(IServiceCollection services)
    {
        try
        {
            _logger.LogInformation("Configuring Swagger...");

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Book Management API",
                    Description = "API for Book's controlling"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter the JWT token using the format: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlFile = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                options.IncludeXmlComments(xmlFile);
            });

            _logger.LogInformation("Swagger configured successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while configuring Swagger.");
            throw;
        }
    }
}
