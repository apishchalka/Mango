using AutoMapper;
using Mango.OrderAPI.Dto.Model;
using Mango.Services.OrderAPI.Dto;
using Mango.Services.OrderAPI.Model;

namespace Mango.Services.OrderAPI.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration Create()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();
                config.CreateMap<CartHeaderDto, OrderHeaderDto>()
                .ForMember(x => x.OrderTotal, y => y.MapFrom(src => src.CartTotal))
                .ReverseMap();

                config.CreateMap<CartDetailsDto, OrderDetailDto>()
                .ForMember(x => x.ProductName, y => y.MapFrom(src => src.Product.Name))
                .ForMember(x => x.ProductPrice, y => y.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailDto, CartDetailsDto>();

            });

            return mappingConfig;

        }
    }
}
