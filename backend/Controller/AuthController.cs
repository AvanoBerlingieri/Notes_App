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
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            // call signup method
            await _authService.SignupAsync(dto);

            // Return success status
            return Ok(new
            {
                message = "User created successfully."
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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

        try
        {
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
            return Ok(new
            {
                response.UserId,
                response.Email,
                response.UserName,
                message = "User authenticated successfully."
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    ///     Deletes jwt from cookie to end user session
    /// </summary>
    /// <returns>Returns 200 with message</returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // delete jwt from cookie
        Response.Cookies.Delete("accessToken");

        return Ok(new
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
        try
        {
            // Grab userId from the claims in the JWT
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            // Call the auth service to retrieve the user data
            var user = await _authService.GetUserAsync(userId);

            // Return the user information with 200 code (ok)
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    ///     Updates the current user's first and last name
    /// </summary>
    /// <param name="dto">DTO containing the updated name</param>
    /// <returns>Returns 200 if the update succeeds</returns>
    [Authorize]
    [HttpPut("user/name")]
    public async Task<IActionResult> UpdateName(UpdateNameDto dto)
    {
        try
        {
            // Grab userId from the claims in the JWT
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            // Call the auth service to update the user's account info
            var updatedUser = await _authService.UpdateNameAsync(userId, dto);

            // Return 200 with user info
            return Ok(updatedUser);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    ///     Updates the current user's email
    /// </summary>
    /// <param name="dto">DTO containing the updated email</param>
    /// <returns>Returns 200 if the update succeeds</returns>
    [Authorize]
    [HttpPut("user/email")]
    public async Task<IActionResult> UpdateEmail(UpdateEmailDto dto)
    {
        try
        {
            // Grab userId from the claims in the JWT
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            // Call the auth service to update the user's account info
            var updatedUser = await _authService.UpdateEmailAsync(userId, dto);

            // Return 200 with user info
            return Ok(updatedUser);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    ///     Changes the password for the current user
    /// </summary>
    /// <param name="dto">DTO containing the current password and the new password</param>
    /// <returns>Returns 204 (No Content) if the password change succeeds</returns>
    [Authorize]
    [HttpPatch("user/pass")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        try
        {
            // Grab userId from the claims in the JWT
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            // Call the auth service to update the user's password
            await _authService.ChangePasswordAsync(userId, dto);

            // Return 204 (No Content) if password successfully changed
            return StatusCode(204);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    ///     Deletes the user
    /// </summary>
    /// <returns>Returns 204 (No Content) if the user was successfully deleted</returns>
    [Authorize]
    [HttpDelete("user")]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            // Grab userId from the claims in the JWT
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            // Call the auth service to delete the user
            await _authService.DeleteUserAsync(userId);

            // Return 204 (No Content) if user deleted successfully
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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