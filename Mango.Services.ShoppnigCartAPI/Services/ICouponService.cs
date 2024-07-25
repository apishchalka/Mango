using Mango.Services.ShoppnigCartAPI.Dto;

namespace Mango.Services.ShoppnigCartAPI.Services
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponAsync(string code);
    }
}
