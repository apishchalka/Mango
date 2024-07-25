using Mango.Services.ShoppnigCartAPI.Dto;
using Mango.Services.ShoppnigCartAPI.Models;

namespace Mango.Services.ShoppnigCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly HttpClient _httpClient;

        public CouponService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CouponDto> GetCouponAsync(string code)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/couponAPI/GetByCode/{code}");
                response.EnsureSuccessStatusCode();
                var responseDto = await response.Content.ReadFromJsonAsync<ResponseDto<CouponDto>>();
                return responseDto.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
