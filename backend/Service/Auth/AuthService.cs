using NotesApp.DTO.Auth;

namespace NotesApp.Service.Auth;

public interface IAuthService
{
    Task SignupAsync(SignupDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto> GetUserAsync(Guid userId);
    Task<UpdateNameDto> UpdateNameAsync(Guid userId, UpdateNameDto dto);
    Task<UpdateEmailDto> UpdateEmailAsync(Guid userId, UpdateEmailDto dto);

    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task DeleteUserAsync(Guid userId);
}