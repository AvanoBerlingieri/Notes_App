using NotesApp.DTO.Auth;

namespace NotesApp.Service;

public interface IAuthService
{
    Task SignupAsync(SignupDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}