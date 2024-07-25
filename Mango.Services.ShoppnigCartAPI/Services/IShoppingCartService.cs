using Mango.Services.ShoppnigCartAPI.Model.Dto;
using Mango.Services.ShoppnigCartAPI.Models;

namespace Mango.Services.ShoppnigCartAPI.Services
{
    public interface IShoppingCartService
    {
        Task<ResponseDto<bool>> ApplyCoupon(ShoppingCartDto cart);
        Task<ResponseDto<bool>> EmailCart(ShoppingCartDto cart);
        Task<ResponseDto<ShoppingCartDto>> GetCart(string userId);
        Task<ResponseDto<bool>> Upsert(ShoppingCartDto shoppingCart);

    }
}
