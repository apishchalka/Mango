using AutoMapper;
using Mango.Services.ShoppnigCartAPI.Model;
using Mango.Services.ShoppnigCartAPI.Model.Dto;

namespace Mango.Services.CouponAPI.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration Create()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<CartHeaderDto, CartHeader>();
                config.CreateMap<CartHeader, CartHeaderDto>();
                config.CreateMap<CartDetailsDto, CartDetails>();
                config.CreateMap<CartDetails, CartDetailsDto>();
            });

            return mappingConfig;

        }
    }
}
