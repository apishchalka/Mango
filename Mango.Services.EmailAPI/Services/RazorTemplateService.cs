using Mango.Services.EmailAPI.Services;
using RazorLight;

public class RazorTemplateService : IRazorTemplateService
{
    private readonly RazorLightEngine _engine;

    public RazorTemplateService()
    {
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorTemplateService))
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderTemplateAsync<T>(string templatePath, T model)
    {
        string template = await File.ReadAllTextAsync(templatePath);
        string result = await _engine.CompileRenderStringAsync(templatePath, template, model);
        return result;
    }
}
