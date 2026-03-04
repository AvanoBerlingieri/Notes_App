using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotesApp.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NotesApp.DTO.Auth;

namespace NotesApp.Service;

/// <summary>
/// Handles authentication-related business logic such as
/// user registration and login, and JWT generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

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
    /// Registers a new user in the system.
    /// Does NOT automatically log the user in.
    /// </summary>
    /// <param name="dto">Signup data transfer object</param>
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
    /// Authenticates a user using either username or email,
    /// verifies password, and returns a JWT token if valid.
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>Authentication response with JWT</returns>
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        User? user;

        // Determine whether input is email or username
        if (IsValidEmail(dto.UserNameOrEmail))
        {
            user = await _userManager.FindByEmailAsync(dto.UserNameOrEmail);
        }
        else
        {
            user = await _userManager.FindByNameAsync(dto.UserNameOrEmail);
        }

        // Fail if user is not found
        if (user == null)
        {
            throw new Exception("Invalid credentials.");
        }

        // Validate password using Identity
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        // Fail if password incorrect
        if (!result.Succeeded)
        {
            throw new Exception("Invalid credentials.");
        }

        // Generate JWT token for authenticated user
        return GenerateAuthResponse(user);
    }

    /// <summary>
    /// Generates a signed JWT token containing user claims.
    /// </summary>
    /// <param name="user">Authenticated user</param>
    /// <returns>Token string and expiration timestamp</returns>
    private (string Token, DateTime Expiration) GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");

        // Define claims embedded inside the token
        var claims = new List<Claim>
        {
            // Subject (standard JWT claim)
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            // Unique user identifier (used for authorization checks)
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            // Username claim
            new Claim(ClaimTypes.Name, user.UserName!),

            // Email claim
            new Claim(JwtRegisteredClaimNames.Email, user.Email!)
        };

        // Create symmetric security key from configured secret
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

        // Create signing credentials using HMAC SHA256
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Set token expiration time for 1 day
        var expiration = DateTime.UtcNow.AddMinutes(
            double.Parse(jwtSettings["1440"]!));

        // Build JWT token object
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        // Return serialized token string
        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }
    
    // Builds the authentication response DTO including JWT and metadata.
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

    /// Validates whether a string is a properly formatted email address.
    private bool IsValidEmail(string input)
    {
        return new EmailAddressAttribute().IsValid(input);
    }
}