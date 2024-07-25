using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITokenProvider tokenProvider;

        public BaseService(IHttpClientFactory clientFactory, ITokenProvider tokenProvider)
        {
            this._clientFactory = clientFactory;
            this.tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto<T>> SendAsync<T>(RequestDto request, bool withBearer = true)
        {
            try
            {
                var httpClient = _clientFactory.CreateClient();
                HttpRequestMessage message = new();

                //token
                message.RequestUri = new Uri(request.Url);

                if (request.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                }

                if (withBearer)
                {
                    message.Headers.Add("Authorization", $"Bearer {tokenProvider.GetToken()}");
                }

                message.Method = request.Method;

                var response = await httpClient.SendAsync(message);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new ResponseDto<T>() { IsSuccess = false, Message = "Unauthorized" };
                    case System.Net.HttpStatusCode.NotFound:
                        return new ResponseDto<T>() { IsSuccess = false, Message = "Not found" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new ResponseDto<T>() { IsSuccess = false, Message = "Access denied" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new ResponseDto<T>() { IsSuccess = false, Message = "Server error" };

                    default: 
                        var responseData = await response.Content.ReadFromJsonAsync<ResponseDto<T>>();
                        return responseData;
                }
            }
            catch (Exception)
            {
                var dto = new ResponseDto<T>
                {
                    IsSuccess = false,
                    Message = "Server error",
                };

                return dto;
            }
        }
    }
}
