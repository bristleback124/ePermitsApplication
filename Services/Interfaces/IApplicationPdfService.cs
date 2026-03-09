namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicationPdfService
    {
        Task<byte[]> GenerateApplicationPdfAsync(int applicationId, string type);
    }
}
