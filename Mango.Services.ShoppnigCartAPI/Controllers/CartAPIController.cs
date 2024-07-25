using Mango.Services.ShoppnigCartAPI.Model.Dto;
using Mango.Services.ShoppnigCartAPI.Models;
using Mango.Services.ShoppnigCartAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppnigCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartAPIController : ControllerBase
    {
        private readonly IShoppingCartService shoppingCartService;
        
        public CartAPIController(IShoppingCartService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto<ShoppingCartDto>> GetCart(string userId)
        {
            return await shoppingCartService.GetCart(userId);
        }

        [HttpPost("Upsert")]
        public async Task<ResponseDto<bool>> Upsert(ShoppingCartDto shoppingCart)
        { 
            return await shoppingCartService.Upsert(shoppingCart);
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto<bool>> ApplyCoupon(ShoppingCartDto shoppingCart)
        {
            return await shoppingCartService.ApplyCoupon(shoppingCart);
        }

        [HttpPost("EmailCart")]
        public async Task<ResponseDto<bool>> EmailCart(ShoppingCartDto shoppingCart)
        {
            return await shoppingCartService.EmailCart(shoppingCart);
        }
    }
}
