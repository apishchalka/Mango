using Mango.Web.Configuration;
using Mango.Web.Models;
using Mango.Web.Service.IService;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {

        private readonly IBaseService baseService;

        public AuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto<object>> AssignRoleAsync(UserRegisterRequestDTO assignRoleRequest)
        {
            return await baseService.SendAsync<object>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = assignRoleRequest,
                Url = MangoConfig.AuthUrlBase + $"/api/AuthAPI/assign_role"
            });
        }

        public async Task<ResponseDto<LoginResponseDTO>> LoginAsync(LoginRequestDTO loginRequest)
        {
            return await baseService.SendAsync<LoginResponseDTO>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = loginRequest,
                Url = MangoConfig.AuthUrlBase + $"/api/AuthAPI/login"
            }, false);
        }

        public async Task<ResponseDto<UserDTO>> RegisterAsync(UserRegisterRequestDTO reqisterUser)
        {
            return await baseService.SendAsync<UserDTO>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = reqisterUser,
                Url = MangoConfig.AuthUrlBase + $"/api/AuthAPI/register"
            });
        }
    }
}
