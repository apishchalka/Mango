using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
        Task<ResponseDto<UserDTO>> RegisterAsync(UserRegisterRequestDTO reqisterUser);
        Task<ResponseDto<object>> AssignRoleAsync(UserRegisterRequestDTO reqisterUser);
    }
}
