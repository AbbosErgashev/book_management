using BookManagement.API.Configurations;
using BookManagement.API.Middelware;
using BookManagement.API.Services;
using BookManagement.DataAccess;
using BookManagement.DataAccess.IServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Fetch connection string from app settings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext with the service container
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();

// Register other services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();

// Register JWT Authentication configuration
builder.Services.AddSingleton<JwtAuthenticationConfig>();

// Configure JWT Authentication
var jwtConfig = builder.Services.BuildServiceProvider().GetRequiredService<JwtAuthenticationConfig>();
jwtConfig.ConfigureJwtAuthentication(builder.Services);


builder.Services.ConfigureSwagger(options => { });

builder.Services.AddSingleton<SwaggerConfig>();
var swaggerConfig = builder.Services.BuildServiceProvider().GetRequiredService<SwaggerConfig>();
swaggerConfig.ConfigureSwagger(builder.Services);

var app = builder.Build();

// Middleware for error handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
