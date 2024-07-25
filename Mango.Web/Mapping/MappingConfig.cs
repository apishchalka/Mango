using AutoMapper;
using Mango.Web.Service.IService.Dto;

namespace Mango.Services.CouponAPI.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration Create()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeaderDto, CheckoutHeaderDto>();
                config.CreateMap<CartDetailsDto, CheckoutDetailDto>()
                .ForMember(x => x.Count, y => y.MapFrom(src => src.Count))
                .ForMember(x => x.Name, y => y.MapFrom(src => src.Product.Name))
                .ForMember(x => x.Price, y => y.MapFrom(src => src.Product.Price));
                
                config.CreateMap<CartDto, CheckoutCartDto>();
            });

            return mappingConfig;
        }
    }
}
