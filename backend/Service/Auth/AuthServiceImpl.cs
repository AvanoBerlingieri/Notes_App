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
        if (user == null) {throw new Exception("Invalid credentials.");}

        // Validate password using Identity
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        // Fail if password incorrect
        if (!result.Succeeded) throw new Exception("Invalid credentials.");

        // Generate JWT token for authenticated user
        return GenerateAuthResponse(user);
    }

    /// <summary>
    /// Retrieves the account information for a specific user.
    /// </summary>
    /// <param name="userId">The unique id of the user</param>
    /// <returns>Returns a DTO containing the user's username, email, first name, and last name</returns>
    /// <exception cref="Exception">Thrown if a user with the specified ID cannot be found</exception>
    public async Task<UserDto> GetUserAsync(Guid userId)
    {
        // retrieve the user using the provided user ID
        var user = await _userManager.FindByIdAsync(userId.ToString());

        // If the user does not exist, throw an exception
        if (user == null) { throw new Exception("User not found."); }

        // Map the user object to a UserDto that can be returned to the client
        return new UserDto
        {
            UserName = user.UserName!,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }

    /// <summary>
    /// Updates the profile information for an existing user.
    /// </summary>
    /// <param name="userId">The unique id of the user</param>
    /// <param name="dto">DTO containing the updated user information</param>
    /// <returns>DTO containing the updated user information</returns>
    /// <exception cref="Exception">
    /// Thrown if the user can't be found, or the update operation fails.
    /// </exception>
    public async Task<UpdateNameDto> UpdateNameAsync(Guid userId, UpdateNameDto dto)
    {
        // Retrieve the user
        var user = await _userManager.FindByIdAsync(userId.ToString());

        // If the user does not exist, throw an exception
        if (user == null) { throw new Exception("User not found."); }

        // Update the user's properties with the values provided in the DTO
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;

        // update user information in the database
        var result = await _userManager.UpdateAsync(user);

        // If the update fails, throw an exception
        if (!result.Succeeded) { throw new Exception("Failed to update user."); }

        // Return the updated user information as a DTO
        var updatedUser = new UpdateNameDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return updatedUser;
    }

    /// <summary>
    /// Updates the email for an existing user.
    /// </summary>
    /// <param name="userId">The unique id of the user</param>
    /// <param name="dto">DTO containing the updated email</param>
    /// <returns>DTO containing the updated email</returns>
    /// <exception cref="Exception">
    /// Thrown if the user can't be found, or the update operation fails.
    /// </exception>
    public async Task<UpdateEmailDto> UpdateEmailAsync(Guid userId, UpdateEmailDto dto)
    {
        // Validate that the email provided in the DTO is in a valid format
        if (!IsValidEmail(dto.CurrentEmail!) || !IsValidEmail(dto.NewEmail!)) 
        { throw new Exception("Invalid email format!"); }

        // Retrieve the user
        var user = await _userManager.FindByIdAsync(userId.ToString());

        // If the user does not exist, throw an exception
        if (user == null) { throw new Exception("User not found."); }

        // if dto current email doesn't match the users current email, throw exception
        if (user.Email != dto.CurrentEmail) { throw new Exception("Current email does not match."); }
        
        // update user email
        var result = await _userManager.SetEmailAsync(user, dto.NewEmail);
        
        // If the update fails, throw an exception
        if (!result.Succeeded) { throw new Exception("Failed to update user."); }

        var updatedUser = new UpdateEmailDto {
            CurrentEmail = dto.CurrentEmail,
            NewEmail = dto.NewEmail
        };

        return updatedUser;
    }

    /// <summary>
    /// Changes the password for a specific user.
    /// </summary>
    /// <param name="userId">The unique id of the user</param>
    /// <param name="dto">DTO containing the user's current password and the new password.</param>
    /// <exception cref="Exception">Thrown if the user cannot be found or if the password change fails</exception>
    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        // Retrieve the user
        var user = await _userManager.FindByIdAsync(userId.ToString());

        // Check if the user exists
        if (user == null) { throw new Exception("User not found."); }

        // Attempt to change the password
        var result = await _userManager.ChangePasswordAsync(
            user,
            dto.CurrentPassword,
            dto.NewPassword
        );

        // If the password change fails, throw an exception
        if (!result.Succeeded) { throw new Exception("Password change failed."); }
    }

    /// <summary>
    /// Delete a user account from the database.
    /// </summary>
    /// <param name="userId">The unique id of the user</param>
    /// <exception cref="Exception">Thrown if the user cannot be found or the delete operation fails</exception>
    public async Task DeleteUserAsync(Guid userId)
    {
        // Find the user
        var user = await _userManager.FindByIdAsync(userId.ToString());

        // If the user does not exist, throw an exception
        if (user == null) { throw new Exception("User not found."); }

        // delete the user account
        var result = await _userManager.DeleteAsync(user);

        // If the delete operation fails, throw an exception
        if (!result.Succeeded) { throw new Exception("Failed to delete user."); }
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
        var jwtDuration = int.Parse(Environment.GetEnvironmentVariable("JWT_DURATION_MINUTES")!);

        // Define claims for the JWT
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
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