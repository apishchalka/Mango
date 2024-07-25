using Mango.Services.EmailAPI.Dto;
using Mango.Services.EmailAPI.Models.Dtos;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task SendCart(ShoppingCartDto cart);
        Task SendRegistrationEmail(UserRegisterRequestDTO user);
    }
}
