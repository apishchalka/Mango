using Mango.Web.Models;
using Mango.Web.Service.IService.Dto;

namespace Mango.Web.Service.IService
{
    public interface IShoppingCartService
    {
        Task<ResponseDto<CartDto>> GetCart(string userId);
        Task<ResponseDto<object>> Upsert(CartDto shoppingCart);
        Task<ResponseDto<bool>> ApplyCoupon(CartDto shoppingCart);
        
        Task<ResponseDto<bool>> EmailCart(CartDto shoppingCart);
    }
}
