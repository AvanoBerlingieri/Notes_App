using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotesApp.Data;
using NotesApp.Hub;
using NotesApp.Model;
using NotesApp.Service.Auth;
using NotesApp.Service.Notes;

// Create the application builder
var builder = WebApplication.CreateBuilder(args);

// Load environment variables from the .env file
Env.Load();

// Read database configuration values from environment variables
var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var name = Environment.GetEnvironmentVariable("DB_NAME");
var user = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

// Build PostgreSQL connection string
var connectionString = $"Host={host};Port={port};Database={name};Username={user};Password={password}";

// Registers controller
builder.Services.AddControllers();

// Enables authorization services
builder.Services.AddAuthorization();

// Registers ApplicationDbContext with PostgreSQL provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Adds ASP.NET Identity system for authentication and user management
builder.Services
    .AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>() // Store identity data inside ApplicationDbContext
    .AddDefaultTokenProviders(); // Enables features such as password reset tokens

// Register authentication service
builder.Services.AddScoped<IAuthService, AuthService>();

// Register note service for handling note business logic
builder.Services.AddScoped<INoteService, NoteService>();

// Configure authentication middleware to use JWT tokens
builder.Services.AddAuthentication(options =>
    {
        // Set JWT as default authentication scheme
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })

    // Configure JWT token validation
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // check if token info is valid
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),

            // Secret key used to sign the token
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)
            )
        };

        // Custom logic to extract JWT from cookies instead of Authorization header
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["accessToken"];
                return Task.CompletedTask;
            }
        };
    });

// Allows the frontend application to communicate with the backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // React frontend
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Required for cookies
        });
});

// Enables SignalR
builder.Services.AddSignalR();

// Build the application
var app = builder.Build();

// app.UseHttpsRedirection(); Uncomment when deploying

// Enable CORS for frontend communication
app.UseCors("AllowFrontend");

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// Map SignalR hub endpoint for real-time collaboration
app.MapHub<NoteHub>("/NoteHub");

if (app.Environment.IsDevelopment()) app.MapOpenApi();

// Start the application
app.Run();