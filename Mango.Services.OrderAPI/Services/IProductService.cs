using Mango.Services.OrderAPI.Dto;

namespace Mango.Services.OrderAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
