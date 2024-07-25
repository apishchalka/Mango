using Mango.Web.Configuration;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Service.IService.Dto;

namespace Mango.Web.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService baseService;

        public ShoppingCartService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto<bool>> ApplyCoupon(CartDto shoppingCart)
        {
            return await baseService.SendAsync<bool>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = shoppingCart,
                Url = MangoConfig.CartUrlBase + $"/api/cartAPI/ApplyCoupon"
            });
        }

        public async Task<ResponseDto<CartDto>> GetCart(string userId)
        {
            return await baseService.SendAsync<CartDto>(new RequestDto
            {
                Method = HttpMethod.Get,
                Url = MangoConfig.CartUrlBase + $"/api/cartAPI/GetCart/{userId}"
            });
        }

        public async Task<ResponseDto<bool>> EmailCart(CartDto shoppingCart)
        {
            return await baseService.SendAsync<bool>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = shoppingCart,
                Url = MangoConfig.CartUrlBase + $"/api/cartAPI/EmailCart"
            });
        }

        public async Task<ResponseDto<object>> Upsert(CartDto shoppingCart)
        {
            return await baseService.SendAsync<object>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = shoppingCart,
                Url = MangoConfig.CartUrlBase + $"/api/cartAPI/Upsert"
            });
        }
    }
}
