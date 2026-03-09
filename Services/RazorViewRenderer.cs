using ePermitsApp.Services.Interfaces;
using RazorLight;

namespace ePermitsApp.Services;

public class RazorViewRenderer : IRazorViewRenderer
{
    private readonly RazorLightEngine _engine;

    public RazorViewRenderer(RazorLightEngine engine)
    {
        _engine = engine;
    }

    public async Task<string> RenderViewToStringAsync<TModel>(string templateName, TModel model)
    {
        return await _engine.CompileRenderAsync(templateName, model);
    }
}
