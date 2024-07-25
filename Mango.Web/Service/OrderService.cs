using Mango.Web.Configuration;
using Mango.Web.Models;
using Mango.Web.Models.Cart;
using Mango.Web.Service.IService;
using Mango.Web.Service.IService.Dto;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService baseService;

        public OrderService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto<OrderHeaderDto>> CreateOrder(CartDto shoppingCart)
        {
            return await baseService.SendAsync<OrderHeaderDto>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = shoppingCart,
                Url = MangoConfig.OrderUrlBase + $"/api/orderAPI/CreateOrder"
            });
        }

        public async Task<ResponseDto<StripeRequestDto>> CreateStripeSession(StripeRequestDto stripeRequest)
        {
            return await baseService.SendAsync<StripeRequestDto>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = stripeRequest,
                Url = MangoConfig.OrderUrlBase + $"/api/orderAPI/CreateStripeSession"
            });
        }

        public async Task<ResponseDto<OrderHeaderDto>> ValidateStripeSessionAsync(int orderId)
        {
            return await baseService.SendAsync<OrderHeaderDto>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = orderId,
                Url = MangoConfig.OrderUrlBase + $"/api/orderAPI/ValidateStripeSession?orderId={orderId}"
            });
        }
    }
}
