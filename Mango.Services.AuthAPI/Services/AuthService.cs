using Mango.MessageBus;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private AppDbContext _dbContext;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator tokenGenerator, IMessageBus messageBus, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
            _messageBus = messageBus;
            _configuration = configuration;
        }
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user  = await _dbContext.Users.FirstOrDefaultAsync(x=>x.UserName == request.Login);
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (user == null || isPasswordValid == false)
            {
                return new LoginResponseDTO()
                {
                    Login = null,
                    Token = ""
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new LoginResponseDTO
            {
                Login = request.Login,
                Token = _tokenGenerator.GenerateToken(user, roles)
            };
        }

        public async Task<string> RegisterAsync(UserRegisterRequestDTO request)
        {
            var user = new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    var createdUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
                    if (createdUser != null)
                    {
                        var message = JsonConvert.SerializeObject(request);
                        await _messageBus.SendMessageAsync(message, _configuration.GetValue<string>("ServiceBus-UserRegisteredQueue")!);

                        return "";
                    }
                }
                else
                {
                    return result.Errors.First().Description;
                }

            }
            catch (Exception)
            {
            }
            
            return "Error encountered";
        }

        public async Task<bool> AssignRoleAsync(string login, string role)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == login);

            if (user != null)
            {
                var isRoleExist = await _roleManager.RoleExistsAsync(role);

                if (isRoleExist == false)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(user, role);

                return true;
            }

            return false;
        }
    }
}
