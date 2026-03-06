using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotesApp.DTO.Auth;
using NotesApp.Model;

namespace NotesApp.Service.Auth;

/// <summary>
///     Handles authentication-related business logic such as user registration and login, and JWT generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    // Injects Identity managers and configuration settings.
    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    /// <summary>
    ///     Registers a new user. Performs uniqueness checks for username and email.
    /// </summary>
    /// <param name="dto">The signup data containing credentials and profile info.</param>
    public async Task SignupAsync(SignupDto dto)
    {
        // Check if username already exists
        var existingUserByName = await _userManager.FindByNameAsync(dto.UserName);
        if (existingUserByName != null)
            throw new Exception("Username already taken.");

        // Check if email already exists
        var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUserByEmail != null)
            throw new Exception("Email already in use.");

        // Create new user entity
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        // Persist user with hashed password (handled by Identity)
        var result = await _userManager.CreateAsync(user, dto.Password);

        // If creation fails, return Identity validation errors
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    /// <summary>
    ///     Validates credentials and generates an authentication response.
    /// </summary>
    /// <param name="dto">The login credentials (email/username + password).</param>
    /// <returns>A DTO containing user info and the generated JWT.</returns>
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        User? user;

        // Determine whether input is email or username
        if (IsValidEmail(dto.UserNameOrEmail))
            user = await _userManager.FindByEmailAsync(dto.UserNameOrEmail);
        else
            user = await _userManager.FindByNameAsync(dto.UserNameOrEmail);

        // Fail if user is not found
        if (user == null) throw new Exception("Invalid credentials.");

        // Validate password using Identity
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        // Fail if password incorrect
        if (!result.Succeeded) throw new Exception("Invalid credentials.");

        // Generate JWT token for authenticated user
        return GenerateAuthResponse(user);
    }

    /// <summary>
    ///     Generates a signed JWT token containing user claims.
    /// </summary>
    /// <param name="user">Authenticated user</param>
    /// <returns>Token string and expiration timestamp</returns>
    private (string Token, DateTime Expiration) GenerateJwtToken(User user)
    {
        // Load JWT configuration from environment variables
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        if (string.IsNullOrEmpty(jwtKey)) throw new Exception("JWT_KEY is not configured.");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        var jwtDuration = int.Parse(Environment.GetEnvironmentVariable("JWT_DURATION_MINUTES"));

        // Define claims for the JWT
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Email, user.Email!)
        };

        // Create signing credentials
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Set token expiration
        var expiration = DateTime.UtcNow.AddMinutes(jwtDuration);

        // Build the JWT token
        var token = new JwtSecurityToken(
            jwtIssuer,
            jwtAudience,
            claims,
            expires: expiration,
            signingCredentials: creds
        );

        // Return serialized token and expiration
        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }

    /// <summary>
    ///     Internal helper to construct the AuthResponseDto from a user entity.
    /// </summary>
    /// <param name="user">The authenticated user entity.</param>
    private AuthResponseDto GenerateAuthResponse(User user)
    {
        var tokenData = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = tokenData.Token,
            Expiration = tokenData.Expiration,
            UserId = user.Id,
            Email = user.Email!,
            UserName = user.UserName!
        };
    }

    /// <summary>
    ///     Validates if a string follows email formatting rules.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    private bool IsValidEmail(string input)
    {
        return new EmailAddressAttribute().IsValid(input);
    }
}