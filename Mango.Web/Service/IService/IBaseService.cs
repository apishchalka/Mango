using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IBaseService
    {
		Task<ResponseDto<TTResponse>> SendAsync<TTResponse>(RequestDto request, bool withBearer = true);

	}
}
