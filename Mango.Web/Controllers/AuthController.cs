using Mango.Web.Enum;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            this._authService = authService;
            this._tokenProvider = tokenProvider;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            return View(loginRequestDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            ResponseDto<LoginResponseDTO> loginResponse = await _authService.LoginAsync(loginRequest);

            if (loginResponse.IsSuccess)
            {
                await SignInUser(loginResponse.Result);
                _tokenProvider.SetToken(loginResponse.Result.Token);
                return RedirectToAction("Index", "Home");
            }
            else 
            {
                ModelState.AddModelError("CustomError", loginResponse.Message);
            }
            
            return View(loginRequest);
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            var roles = new List<SelectListItem>
            {
               new SelectListItem { Text = Role.Customer.ToString(), Value = Role.Customer.ToString() }               
            };

            ViewBag.Roles = roles;

            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterRequestDTO request)
        {
            if (ModelState.IsValid)
            {
                var registerResponse = await _authService.RegisterAsync(request);

                if (registerResponse != null && registerResponse.IsSuccess)
                {
                    if (string.IsNullOrEmpty(request.Role))
                    {
                        request.Role = Role.Customer.ToString();
                    }

                    var assignRoleResponse = await _authService.AssignRoleAsync(request);

                    if (assignRoleResponse != null && assignRoleResponse.IsSuccess)
                    {
                        TempData["success"] = "Registered successfully.";
                        return RedirectToAction(nameof(Login));
                    }
                }
                else if (!string.IsNullOrEmpty(registerResponse?.Message))
                {
                    TempData["error"] = registerResponse.Message;
                }
            }

            var roles = new List<SelectListItem>
            {
                new SelectListItem { Text = Role.Customer.ToString(), Value = Role.Customer.ToString() },
                new SelectListItem {Text = Role.Administrator.ToString(), Value = Role.Administrator.ToString() }
            };

            ViewBag.Roles = roles;

            return View(request);
        }


        private async Task SignInUser(LoginResponseDTO loginResponse)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.ReadJwtToken(loginResponse.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Email, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, token.Claims.First (x => x.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
