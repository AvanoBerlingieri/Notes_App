using NotesApp.DTO.Auth;

namespace NotesApp.Service.Auth;

public interface IAuthService
{
    Task SignupAsync(SignupDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}