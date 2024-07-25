namespace Mango.Services.EmailAPI.Services
{
    public interface IRazorTemplateService
    {
        Task<string> RenderTemplateAsync<T>(string templatePath, T model);
    }
}
