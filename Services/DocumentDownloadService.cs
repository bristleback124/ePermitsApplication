using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class DocumentDownloadService : IDocumentDownloadService
    {
        private readonly IApplicationService _applicationService;

        public DocumentDownloadService(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public async Task<List<(string Folder, string FileName, string FilePath)>?> GetDocumentPathsAsync(int applicationId, string type)
        {
            if (type == "BuildingPermit")
                return await GetBuildingPermitDocumentsAsync(applicationId);
            if (type == "COO")
                return await GetCoODocumentsAsync(applicationId);
            return null;
        }

        private async Task<List<(string Folder, string FileName, string FilePath)>?> GetBuildingPermitDocumentsAsync(int applicationId)
        {
            var app = await _applicationService.GetApplicationBuildingPermitById(applicationId);
            if (app == null) return null;

            var files = new List<(string Folder, string FileName, string FilePath)>();
            var usedNames = new Dictionary<string, int>();

            // Required documents
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocProofOwnership);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocBarangayClearance);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocTaxDeclaration);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocRealPropTaxReceipt);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocECCorCNC);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocSpecialClearances);

            // Technical documents
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocIoCPlans);
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocSEPlans);
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocEEPlans);
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocSPPlans);
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocBOMCost);
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocSoW);
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocMEPlans);
            AddFiles(files, usedNames, "technical-docs", app.BuildingPermitTechDocs.TechDocECEPlans);

            return files;
        }

        private async Task<List<(string Folder, string FileName, string FilePath)>?> GetCoODocumentsAsync(int applicationId)
        {
            var app = await _applicationService.GetApplicationCoOById(applicationId);
            if (app == null) return null;

            var files = new List<(string Folder, string FileName, string FilePath)>();
            var usedNames = new Dictionary<string, int>();

            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocBldgPermitSPlans);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocAsBuiltPlans);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocConsLogbook);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocConsPhotos);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocBrgyClearance);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocFSIC);
            AddFile(files, usedNames, "required-docs", app.RequiredDocs.ReqDocOthers);

            return files;
        }

        private void AddFiles(List<(string Folder, string FileName, string FilePath)> files,
            Dictionary<string, int> usedNames, string folder, List<FileMetadataDto>? items)
        {
            if (items == null) return;
            foreach (var item in items)
                AddFile(files, usedNames, folder, item);
        }

        private void AddFile(List<(string Folder, string FileName, string FilePath)> files,
            Dictionary<string, int> usedNames, string folder, FileMetadataDto? file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.Path))
                return;

            var fileName = string.IsNullOrWhiteSpace(file.Name)
                ? Path.GetFileName(file.Path)
                : file.Name;

            // Handle duplicate filenames within the same folder
            var key = $"{folder}/{fileName}".ToLowerInvariant();
            if (usedNames.TryGetValue(key, out var count))
            {
                usedNames[key] = count + 1;
                var ext = Path.GetExtension(fileName);
                var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                fileName = $"{nameWithoutExt} ({count + 1}){ext}";
            }
            else
            {
                usedNames[key] = 1;
            }

            files.Add((folder, fileName, file.Path));
        }
    }
}
