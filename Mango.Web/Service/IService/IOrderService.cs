using Mango.Web.Models;
using Mango.Web.Models.Cart;
using Mango.Web.Service.IService.Dto;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto<OrderHeaderDto>> CreateOrder(CartDto shoppingCart);
        Task<ResponseDto<OrderHeaderDto>> ValidateStripeSessionAsync(int orderId);
        Task<ResponseDto<StripeRequestDto>> CreateStripeSession(StripeRequestDto stripeRequest);
    }
}
