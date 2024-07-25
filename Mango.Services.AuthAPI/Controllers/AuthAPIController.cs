using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthAPIController : ControllerBase
    {
        private ResponseDto _responseDto;
        private readonly IAuthService authService;

        public AuthAPIController(IAuthService authService)
        {
            this.authService = authService;
            _responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDTO reqisterRequest)
        {
            var response = await authService.RegisterAsync(reqisterRequest);

            if (!string.IsNullOrEmpty(response))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = response;
                return BadRequest(_responseDto);
            }
            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var userSignInResult = await authService.LoginAsync(loginRequest);

            if (userSignInResult.Login == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Either login or password is incorrect";
                return BadRequest(_responseDto);
            }

            _responseDto.Result = userSignInResult;

            return Ok(_responseDto);
        }

        [HttpPost("assign_role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRegisterRequestDTO request)
        {

            if (string.IsNullOrEmpty(request.Role))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Role is required";
                return BadRequest(_responseDto);
            }

            var userSignInResult = await authService.AssignRoleAsync(request.Email, request.Role);

            if (!userSignInResult)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = $"Could not assign role {request.Role} to user {request.Name}.";
                return BadRequest(_responseDto);
            }

            return Ok(_responseDto);
        }
    }
}
