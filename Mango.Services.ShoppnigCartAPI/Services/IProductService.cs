using Mango.Services.ShoppnigCartAPI.Model.Dto;

namespace Mango.Services.ShoppnigCartAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
