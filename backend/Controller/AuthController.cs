using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.DTO.Auth;
using NotesApp.Service.Auth;

namespace NotesApp.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    ///     Injects the authentication service into the controller.
    /// </summary>
    /// <param name="authService">Service handling authentication logic</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Registers a new user account.
    /// </summary>
    /// <param name="dto">Signup data transfer object containing user credentials</param>
    /// <returns>
    ///     201 Created on success, along with a success message.
    ///     400 Bad Request if the model validation fails.
    /// </returns>
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupDto dto)
    {
        // Validate request payload
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // call signup method
        await _authService.SignupAsync(dto);

        // Return success status
        return StatusCode(201, new
        {
            message = "User created successfully."
        });
    }

    /// <summary>
    ///     Authenticates an existing user using username or email and password.
    ///     Returns a JWT token for subsequent requests if credentials are valid.
    /// </summary>
    /// <param name="dto">Login data transfer object</param>
    /// <returns>
    ///     200 OK with AuthResponseDto containing JWT token, expiration, user ID, email, and username.
    ///     400 Bad Request if model validation fails.
    ///     401 Unauthorized if credentials are invalid.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Validate request payload
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // call login method
        var response = await _authService.LoginAsync(dto);

        // Creating the cookie
        Response.Cookies.Append("accessToken", response.Token, new CookieOptions
        {
            HttpOnly = true, // prevents XSS scripts from stealing the token
            // Secure = true, uncomment when going into prod
            SameSite = SameSiteMode.Lax,
            Expires = response.Expiration
        });

        // Return authentication response
        return StatusCode(200, new
        {
            response.UserId,
            response.Email,
            response.UserName,
            message = "User authenticated successfully."
        });
    }

    /// <summary>
    ///     Deletes jwt from cookie to end user session
    /// </summary>
    /// <returns>Returns 204 (No Content)</returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // delete jwt from cookie
        Response.Cookies.Delete("accessToken");

        return StatusCode(200, new
        {
            message = "Logged out successfully."
        });
    }

    /// <summary>
    ///     Retrieves the currently authenticated user's profile information.
    /// </summary>
    /// <returns>Returns the user's account information</returns>
    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // Call the auth service to retrieve the user data
        var user = await _authService.GetUserAsync(userId);

        // Return the user information with 200 code (ok)
        return StatusCode(200, user);
    }

    /// <summary>
    ///     Updates the current user's profile information
    /// </summary>
    /// <param name="dto">DTO containing the updated user info</param>
    /// <returns>Returns 204 (No Content) if the update succeeds</returns>
    [Authorize]
    [HttpPut("user")]
    public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // Call the auth service to update the user's account info
        await _authService.UpdateUserAsync(userId, dto);

        // Return 204 (No Content)
        return StatusCode(204);
    }

    /// <summary>
    ///     Changes the password for the current user
    /// </summary>
    /// <param name="dto">DTO containing the current password and the new password</param>
    /// <returns>Returns 204 (No Content) if the password change succeeds</returns>
    [Authorize]
    [HttpPatch("user")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // Call the auth service to update the user's password
        await _authService.ChangePasswordAsync(userId, dto);

        // Return 204 (No Content) if password successfully changed
        return StatusCode(204);
    }

    /// <summary>
    ///     Deletes the user
    /// </summary>
    /// <returns>Returns 204 (No Content) if the user was successfully deleted</returns>
    public async Task<IActionResult> DeleteUser()
    {
        // Grab userId from the claims in the JWT
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        // Call the auth service to delete the user
        await _authService.DeleteUserAsync(userId);

        // Return 204 (No Content) if user deleted successfully
        return StatusCode(204, new
        {
            message = "User deleted successfully."
        });
    }

    /// <summary>
    ///     Helper function to grab userId from jwt claims
    /// </summary>
    /// <returns>User id</returns>
    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}