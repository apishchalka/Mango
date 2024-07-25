using Mango.Services.OrderAPI.Dto;

namespace Mango.Services.OrderAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/productAPI");
                response.EnsureSuccessStatusCode();
                var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto<IEnumerable<ProductDto>>>();
                return responseDto.Result;
            }
            catch (Exception)
            {
                return Enumerable.Empty<ProductDto>();
            }
        }
    }
}
