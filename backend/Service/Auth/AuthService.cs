using NotesApp.DTO.Auth;

namespace NotesApp.Service.Auth;

public interface IAuthService
{
    Task SignupAsync(SignupDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto> GetUserAsync(Guid userId);
    Task<UpdateUserDto> UpdateUserAsync(Guid userId, UpdateUserDto dto);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task DeleteUserAsync(Guid userId);
}