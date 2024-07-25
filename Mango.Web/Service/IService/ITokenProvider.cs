namespace Mango.Web.Service.IService
{
    public interface ITokenProvider
    {
        void SetToken(string token);
        void ClearToken();
        string GetToken();
    }
}
