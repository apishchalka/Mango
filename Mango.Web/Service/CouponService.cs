using Mango.Web.Configuration;
using Mango.Web.Models;
using Mango.Web.Service.IService;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService baseService;

        public CouponService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto<IEnumerable<CouponDto>>> GetAllAsync()
        {
            return await baseService.SendAsync<IEnumerable<CouponDto>>(new RequestDto
            {
                Method = HttpMethod.Get,
                Url = MangoConfig.CouponUrlBase + $"/api/couponAPI"
            });
        }

        public async Task<ResponseDto<object>> DeleteAsync(int id)
        {
            return await baseService.SendAsync<object>(new RequestDto
            {
                Method = HttpMethod.Delete,
                Url = MangoConfig.CouponUrlBase + $"/api/couponAPI/{id}"
            });
        }

        public async Task<ResponseDto<CouponDto>> CreateAsync(CouponDto newCoupon)
        {
            return await baseService.SendAsync<CouponDto>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = newCoupon,
                Url = MangoConfig.CouponUrlBase + $"/api/couponAPI"
            });
        }        
    }
}
