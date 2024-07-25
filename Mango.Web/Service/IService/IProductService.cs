using Mango.Web.Models;
namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
		Task<ResponseDto<IEnumerable<ProductDto>>> GetAllAsync();
        Task<ResponseDto<ProductDto>> GetByIdAsync(int id);
        Task<ResponseDto<ProductDto>> CreateAsync(ProductDto product);
        Task<ResponseDto<object>> DeleteAsync(int productId);
    }
}
