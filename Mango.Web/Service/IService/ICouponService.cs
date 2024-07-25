using Mango.Web.Models;
namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
		Task<ResponseDto<IEnumerable<CouponDto>>> GetAllAsync();
        Task<ResponseDto<CouponDto>> CreateAsync(CouponDto newCoupon);
        Task<ResponseDto<object>> DeleteAsync(int couponId);
    }
}
