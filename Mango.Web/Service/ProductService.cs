using Mango.Web.Configuration;
using Mango.Web.Models;
using Mango.Web.Service.IService;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService baseService;

        public ProductService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDto<IEnumerable<ProductDto>>> GetAllAsync()
        {
            return await baseService.SendAsync<IEnumerable<ProductDto>>(new RequestDto
            {
                Method = HttpMethod.Get,
                Url = MangoConfig.ProductUrlBase + $"/api/productAPI"
            });
        }

        public async Task<ResponseDto<object>> DeleteAsync(int id)
        {
            return await baseService.SendAsync<object>(new RequestDto
            {
                Method = HttpMethod.Delete,
                Url = MangoConfig.ProductUrlBase + $"/api/productAPI/{id}"
            });
        }

        public async Task<ResponseDto<ProductDto>> CreateAsync(ProductDto newProduct)
        {
            return await baseService.SendAsync<ProductDto>(new RequestDto
            {
                Method = HttpMethod.Post,
                Data = newProduct,
                Url = MangoConfig.ProductUrlBase + $"/api/productAPI"
            });
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(int id)
        {
            return await baseService.SendAsync<ProductDto>(new RequestDto
            {
                Method = HttpMethod.Get,
                Url = MangoConfig.ProductUrlBase + $"/api/productAPI/{id}"
            });
        }
    }
}
