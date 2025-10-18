using LigaLibre.Application.DTOs;

namespace LigaLibre.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<string> GenerateJwtTokenAsync(string email);
    }
}
