namespace ePermitsApp.Services.Interfaces
{
    public interface IDocumentDownloadService
    {
        Task<List<(string Folder, string FileName, string FilePath)>?> GetDocumentPathsAsync(int applicationId, string type);
    }
}
