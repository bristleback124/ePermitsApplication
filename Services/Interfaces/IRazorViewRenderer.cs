namespace ePermitsApp.Services.Interfaces;

public interface IRazorViewRenderer
{
    Task<string> RenderViewToStringAsync<TModel>(string templateName, TModel model);
}
