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
    /// Injects the authentication service into the controller.
    /// </summary>
    /// <param name="authService">Service handling authentication logic</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="dto">Signup data transfer object containing user credentials</param>
    /// <returns>
    /// 201 Created on success, along with a success message.  
    /// 400 Bad Request if the model validation fails.
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
        return StatusCode(201, new {
            message = "User created successfully."
        });
    }

    /// <summary>
    /// Authenticates an existing user using username or email and password.
    /// Returns a JWT token for subsequent requests if credentials are valid.
    /// </summary>
    /// <param name="dto">Login data transfer object</param>
    /// <returns>
    /// 200 OK with AuthResponseDto containing JWT token, expiration, user ID, email, and username.  
    /// 400 Bad Request if model validation fails.  
    /// 401 Unauthorized if credentials are invalid.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Validate request payload
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // call login method
        var response = await _authService.LoginAsync(dto);
        
        Response.Cookies.Append("accessToken", response.Token, new CookieOptions
        {
            HttpOnly = true,
            // Secure = true, uncomment when going into prod
            SameSite = SameSiteMode.Lax,
            Expires = response.Expiration
        });

        // Return authentication response
        return StatusCode(200, new {
            response.UserId,
            response.Email,
            response.UserName,
            message = "User authenticated successfully."
        });
    }

    /// <summary>
    /// Deletes jwt from cookie to end user session
    /// </summary>
    /// <returns>Returns 200 status code</returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // delete jwt from cookie
        Response.Cookies.Delete("accessToken");

        return StatusCode(200, new {
            message = "Logged out successfully."
        });
    }
}