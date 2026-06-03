using DsGestor.Application.Dtos;

namespace DsGestor.Application.Services;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(UserRegisterDto dto);
    Task<UserResponseDto> LoginAsync(UserLoginDto dto);
}
