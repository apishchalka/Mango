using AutoMapper;
using Mango.Services.CouponAPI.Dto;
using Mango.Services.CouponAPI.Models;

namespace Mango.Services.CouponAPI.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration Create()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<Coupon, CouponDto>();
                config.CreateMap<CouponDto, Coupon>();
            });

            return mappingConfig;

        }
    }
}
