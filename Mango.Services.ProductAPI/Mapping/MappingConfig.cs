using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;

namespace Mango.Services.CouponAPI.Mapping
{
    public class MappingConfig
    {
        public static MapperConfiguration Create()
        {
            var mappingConfig = new MapperConfiguration(config => {
                config.CreateMap<Product, ProductDto>();
                config.CreateMap<ProductDto, Product>();
            });

            return mappingConfig;

        }
    }
}
