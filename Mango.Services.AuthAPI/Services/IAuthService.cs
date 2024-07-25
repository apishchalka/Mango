using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserRegisterRequestDTO request);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);

        Task<bool> AssignRoleAsync(string login, string role);
    }
}
