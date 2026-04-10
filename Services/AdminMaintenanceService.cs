using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Entities.CoOApp;
using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Services
{
    public class AdminMaintenanceService : IAdminMaintenanceService
    {
        private const string PermitApplicationTypes = "permit-application-types";
        private const string ProjectClassifications = "project-classifications";
        private const string OwnershipTypes = "ownership-types";
        private const string OccupancyNatures = "occupancy-natures";
        private const string ApplicantTypes = "applicant-types";
        private const string BuildingPermitCategories = "building-permit-categories";
        private const string Provinces = "provinces";
        private const string Lgus = "lgus";
        private const string Barangays = "barangays";
        private const string RequirementClassifications = "requirement-classifications";
        private const string RequirementCategories = "requirement-categories";
        private const string Requirements = "requirements";
        private const string CertificateOfOccupancyRequirementsClassification = "Certificate of Occupancy Requirements";
        private const string CertificateOfOccupancyGeneralCategory = "General Requirements";

        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AdminMaintenanceService(
            ApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<MaintenanceLookupItemDto>> GetLookupItemsAsync(
            string entityType,
            bool includeInactive,
            bool includeDeleted,
            string? search,
            string? applicationType)
        {
            var normalizedEntityType = NormalizeEntityType(entityType);
            var normalizedSearch = search?.Trim();
            var normalizedApplicationType = NormalizeOptionalApplicationType(applicationType);

            return normalizedEntityType switch
            {
                PermitApplicationTypes => await GetPermitApplicationTypesAsync(includeInactive, includeDeleted, normalizedSearch),
                ProjectClassifications => await GetProjectClassificationsAsync(includeInactive, includeDeleted, normalizedSearch),
                OwnershipTypes => await GetOwnershipTypesAsync(includeInactive, includeDeleted, normalizedSearch),
                OccupancyNatures => await GetOccupancyNaturesAsync(includeInactive, includeDeleted, normalizedSearch),
                ApplicantTypes => await GetApplicantTypesAsync(includeInactive, includeDeleted, normalizedSearch),
                BuildingPermitCategories => await GetBuildingPermitCategoriesAsync(includeInactive, includeDeleted, normalizedSearch),
                Provinces => await GetProvincesAsync(includeInactive, includeDeleted, normalizedSearch),
                Lgus => await GetLgusAsync(includeInactive, includeDeleted, normalizedSearch),
                Barangays => await GetBarangaysAsync(includeInactive, includeDeleted, normalizedSearch),
                RequirementClassifications => await GetRequirementClassificationsAsync(includeInactive, includeDeleted, normalizedSearch, normalizedApplicationType),
                RequirementCategories => await GetRequirementCategoriesAsync(includeInactive, includeDeleted, normalizedSearch, normalizedApplicationType),
                Requirements => await GetRequirementsAsync(includeInactive, includeDeleted, normalizedSearch, normalizedApplicationType),
                _ => throw new InvalidOperationException($"Unsupported maintenance entity type '{entityType}'.")
            };
        }

        public async Task<MaintenanceLookupItemDto> SetActiveStatusAsync(string entityType, int id, bool isActive)
        {
            var normalizedEntityType = NormalizeEntityType(entityType);
            var now = DateTime.UtcNow;
            var actor = _currentUser.UserName ?? "System";

            MaintenanceLookupItemDto result = normalizedEntityType switch
            {
                PermitApplicationTypes => await SetSimpleStatusAsync(
                    _context.PermitApplicationTypes.IgnoreQueryFilters(),
                    id,
                    item => item.PermitAppTypeDesc,
                    normalizedEntityType,
                    isActive,
                    now,
                    actor),
                ProjectClassifications => await SetSimpleStatusAsync(
                    _context.ProjectClassifications.IgnoreQueryFilters(),
                    id,
                    item => item.ProjectClassDesc,
                    normalizedEntityType,
                    isActive,
                    now,
                    actor),
                OwnershipTypes => await SetSimpleStatusAsync(
                    _context.OwnershipTypes.IgnoreQueryFilters(),
                    id,
                    item => item.OwnershipTypeDesc,
                    normalizedEntityType,
                    isActive,
                    now,
                    actor),
                BuildingPermitCategories => await SetSimpleStatusAsync(
                    _context.BuildingPermitCategories.IgnoreQueryFilters(),
                    id,
                    item => item.CategoryName,
                    normalizedEntityType,
                    isActive,
                    now,
                    actor),
                OccupancyNatures => await SetSimpleStatusAsync(
                    _context.OccupancyNatures.IgnoreQueryFilters(),
                    id,
                    item => item.OccupancyNatureDesc,
                    normalizedEntityType,
                    isActive,
                    now,
                    actor),
                ApplicantTypes => await SetSimpleStatusAsync(
                    _context.ApplicantTypes.IgnoreQueryFilters(),
                    id,
                    item => item.ApplicantTypeDesc,
                    normalizedEntityType,
                    isActive,
                    now,
                    actor),
                Provinces => await SetProvinceStatusAsync(id, isActive, now, actor),
                Lgus => await SetLguStatusAsync(id, isActive, now, actor),
                Barangays => await SetBarangayStatusAsync(id, isActive, now, actor),
                RequirementClassifications => await SetRequirementClassificationStatusAsync(id, isActive, now, actor),
                RequirementCategories => await SetRequirementCategoryStatusAsync(id, isActive, now, actor),
                Requirements => await SetRequirementStatusAsync(id, isActive, now, actor),
                _ => throw new InvalidOperationException($"Unsupported maintenance entity type '{entityType}'.")
            };

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<MaintenanceReferenceUsageDto> GetReferenceUsageAsync(string entityType, int id)
        {
            var normalizedEntityType = NormalizeEntityType(entityType);

            return normalizedEntityType switch
            {
                PermitApplicationTypes => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.PermitApplicationTypes.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.PermitAppTypeDesc).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permits", _context.BuildingPermits.CountAsync(x => x.PermitAppTypeId == id))
                    }),
                ProjectClassifications => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.ProjectClassifications.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.ProjectClassDesc).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permits", _context.BuildingPermits.CountAsync(x => x.ProjectClassId == id))
                    }),
                OwnershipTypes => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.OwnershipTypes.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.OwnershipTypeDesc).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permit applicant info", _context.BuildingPermitAppInfos.CountAsync(x => x.OwnershipTypeId == id))
                    }),
                OccupancyNatures => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.OccupancyNatures.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.OccupancyNatureDesc).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permits", _context.BuildingPermits.CountAsync(x => x.OccupancyNatureId == id)),
                        await BuildUsageItemAsync("Certificate of occupancy", _context.CoOApps.CountAsync(x => x.OccupancyNatureId == id))
                    }),
                ApplicantTypes => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.ApplicantTypes.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.ApplicantTypeDesc).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permit applicant info", _context.BuildingPermitAppInfos.CountAsync(x => x.ApplicantTypeId == id)),
                        await BuildUsageItemAsync("Certificate of occupancy", _context.CoOApps.CountAsync(x => x.ApplicantTypeId == id))
                    }),
                BuildingPermitCategories => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.BuildingPermitCategories.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.CategoryName).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permits", _context.BuildingPermits.CountAsync(x => x.BuildingPermitCategoryId == id)),
                        await BuildUsageItemAsync("Requirements", _context.Requirements.IgnoreQueryFilters().CountAsync(x => x.BuildingPermitCategoryId == id))
                    }),
                Provinces => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.Provinces.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.ProvinceName).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permits", _context.BuildingPermits.CountAsync(x => x.ProvinceId == id)),
                        await BuildUsageItemAsync("Certificate of occupancy", _context.CoOApps.CountAsync(x => x.ProvinceId == id)),
                        await BuildUsageItemAsync("Municipalities", _context.LGUs.IgnoreQueryFilters().CountAsync(x => x.ProvinceId == id))
                    }),
                Lgus => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.LGUs.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.LGUName).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permits", _context.BuildingPermits.CountAsync(x => x.LGUId == id)),
                        await BuildUsageItemAsync("Certificate of occupancy", _context.CoOApps.CountAsync(x => x.LGUId == id)),
                        await BuildUsageItemAsync("Barangays", _context.Barangays.IgnoreQueryFilters().CountAsync(x => x.LGUId == id))
                    }),
                Barangays => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.Barangays.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.BarangayName).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Building permits", _context.BuildingPermits.CountAsync(x => x.BarangayId == id)),
                        await BuildUsageItemAsync("Certificate of occupancy", _context.CoOApps.CountAsync(x => x.BarangayId == id))
                    }),
                RequirementClassifications => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.RequirementClassifications.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.ReqClassDesc).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Requirement categories", _context.RequirementCategorys.IgnoreQueryFilters().CountAsync(x => x.ReqClassId == id)),
                        await BuildUsageItemAsync("Requirements", _context.Requirements.IgnoreQueryFilters().CountAsync(x => x.RequirementCategory.ReqClassId == id))
                    }),
                RequirementCategories => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.RequirementCategorys.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.ReqCatDesc).FirstOrDefaultAsync(),
                    new[]
                    {
                        await BuildUsageItemAsync("Requirements", _context.Requirements.IgnoreQueryFilters().CountAsync(x => x.ReqCatId == id))
                    }),
                Requirements => await BuildUsageAsync(
                    normalizedEntityType,
                    id,
                    await _context.Requirements.IgnoreQueryFilters().Where(x => x.Id == id).Select(x => x.ReqDesc).FirstOrDefaultAsync(),
                    Array.Empty<MaintenanceReferenceUsageItemDto>()),
                _ => throw new InvalidOperationException($"Unsupported maintenance entity type '{entityType}'.")
            };
        }

        public async Task<object> CreateItemAsync(string entityType, IDictionary<string, object?> values)
        {
            var normalizedEntityType = NormalizeEntityType(entityType);
            var name = RequiredString(values, "name");
            var parentId = OptionalInt(values, "parentId");
            var scope = NormalizeOptionalScope(values, "applicationTypeScope");
            var buildingPermitCategoryId = OptionalInt(values, "buildingPermitCategoryId");

            return normalizedEntityType switch
            {
                PermitApplicationTypes => await CreateSimpleItemAsync(
                    _context.PermitApplicationTypes.IgnoreQueryFilters().AnyAsync(x => x.PermitAppTypeDesc == name),
                    () => _context.PermitApplicationTypes.Add(new PermitApplicationType
                    {
                        PermitAppTypeDesc = name,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserName ?? "System"
                    }),
                    async () => (object)await _context.PermitApplicationTypes.IgnoreQueryFilters()
                        .Where(x => x.PermitAppTypeDesc == name)
                        .Select(x => new { x.Id, Name = x.PermitAppTypeDesc, x.IsActive })
                        .FirstAsync(),
                    "Type of application"),
                ProjectClassifications => await CreateSimpleItemAsync(
                    _context.ProjectClassifications.IgnoreQueryFilters().AnyAsync(x => x.ProjectClassDesc == name),
                    () => _context.ProjectClassifications.Add(new ProjectClassification
                    {
                        ProjectClassDesc = name,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserName ?? "System"
                    }),
                    async () => (object)await _context.ProjectClassifications.IgnoreQueryFilters()
                        .Where(x => x.ProjectClassDesc == name)
                        .Select(x => new { x.Id, Name = x.ProjectClassDesc, x.IsActive })
                        .FirstAsync(),
                    "Project classification"),
                OwnershipTypes => await CreateSimpleItemAsync(
                    _context.OwnershipTypes.IgnoreQueryFilters().AnyAsync(x => x.OwnershipTypeDesc == name),
                    () => _context.OwnershipTypes.Add(new OwnershipType
                    {
                        OwnershipTypeDesc = name,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserName ?? "System"
                    }),
                    async () => (object)await _context.OwnershipTypes.IgnoreQueryFilters()
                        .Where(x => x.OwnershipTypeDesc == name)
                        .Select(x => new { x.Id, Name = x.OwnershipTypeDesc, x.IsActive })
                        .FirstAsync(),
                    "Ownership type"),
                BuildingPermitCategories => await CreateSimpleItemAsync(
                    _context.BuildingPermitCategories.IgnoreQueryFilters().AnyAsync(x => x.CategoryName == name),
                    () => _context.BuildingPermitCategories.Add(new BuildingPermitCategory
                    {
                        CategoryName = name,
                        Description = string.Empty,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserName ?? "System"
                    }),
                    async () => (object)await _context.BuildingPermitCategories.IgnoreQueryFilters()
                        .Where(x => x.CategoryName == name)
                        .Select(x => new { x.Id, Name = x.CategoryName, x.IsActive })
                        .FirstAsync(),
                    "Building permit category"),
                OccupancyNatures => await CreateSimpleItemAsync(
                    _context.OccupancyNatures.IgnoreQueryFilters().AnyAsync(x => x.OccupancyNatureDesc == name),
                    () => _context.OccupancyNatures.Add(new OccupancyNature
                    {
                        OccupancyNatureDesc = name,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserName ?? "System"
                    }),
                    async () => (object)await _context.OccupancyNatures.IgnoreQueryFilters()
                        .Where(x => x.OccupancyNatureDesc == name)
                        .Select(x => new { x.Id, Name = x.OccupancyNatureDesc, x.IsActive })
                        .FirstAsync(),
                    "Occupancy classification"),
                ApplicantTypes => await CreateSimpleItemAsync(
                    _context.ApplicantTypes.IgnoreQueryFilters().AnyAsync(x => x.ApplicantTypeDesc == name),
                    () => _context.ApplicantTypes.Add(new ApplicantType
                    {
                        ApplicantTypeDesc = name,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserName ?? "System"
                    }),
                    async () => (object)await _context.ApplicantTypes.IgnoreQueryFilters()
                        .Where(x => x.ApplicantTypeDesc == name)
                        .Select(x => new { x.Id, Name = x.ApplicantTypeDesc, x.IsActive })
                        .FirstAsync(),
                    "Applicant type"),
                Provinces => await CreateSimpleItemAsync(
                    _context.Provinces.IgnoreQueryFilters().AnyAsync(x => x.ProvinceName == name),
                    () => _context.Provinces.Add(new Province
                    {
                        ProvinceName = name,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserName ?? "System"
                    }),
                    async () => (object)await _context.Provinces.IgnoreQueryFilters()
                        .Where(x => x.ProvinceName == name)
                        .Select(x => new { x.Id, Name = x.ProvinceName, x.IsActive })
                        .FirstAsync(),
                    "Province"),
                Lgus => await CreateLguAsync(name, parentId),
                Barangays => await CreateBarangayAsync(name, parentId),
                RequirementClassifications => await CreateRequirementClassificationAsync(name, scope, buildingPermitCategoryId),
                RequirementCategories => await CreateRequirementCategoryAsync(name, parentId, scope, buildingPermitCategoryId),
                Requirements => await CreateRequirementAsync(name, parentId, scope, buildingPermitCategoryId),
                _ => throw new InvalidOperationException($"Unsupported maintenance entity type '{entityType}'.")
            };
        }

        public async Task<MaintenanceImportResultDto> ImportAsync(MaintenanceImportRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                throw new InvalidOperationException("Import file is required.");

            var result = new MaintenanceImportResultDto();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            await using var copy = new MemoryStream();
            await request.File.CopyToAsync(copy);
            copy.Position = 0;

            using var document = SpreadsheetDocument.Open(copy, false);
            var workbookPart = document.WorkbookPart
                ?? throw new InvalidOperationException("Workbook part is missing.");

            var actor = _currentUser.UserName ?? "System";
            var now = DateTime.UtcNow;

            foreach (var sheet in workbookPart.Workbook.Sheets!.Elements<Sheet>())
            {
                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
                var rows = ReadWorksheetRows(workbookPart, worksheetPart).ToList();
                if (rows.Count <= 1)
                    continue;

                var headers = rows[0];
                for (var rowIndex = 1; rowIndex < rows.Count; rowIndex++)
                {
                    var rowNumber = rowIndex + 1;
                    var rowValues = rows[rowIndex];
                    if (rowValues.All(string.IsNullOrWhiteSpace))
                        continue;

                    var rowData = MapRow(headers, rowValues);

                    try
                    {
                        switch (sheet.Name?.Value)
                        {
                            case "PermitApplicationTypes":
                                await UpsertPermitApplicationTypeAsync(rowData, now, actor, result);
                                break;
                            case "ProjectClassifications":
                                await UpsertProjectClassificationAsync(rowData, now, actor, result);
                                break;
                            case "OwnershipTypes":
                                await UpsertOwnershipTypeAsync(rowData, now, actor, result);
                                break;
                            case "OccupancyNatures":
                                await UpsertOccupancyNatureAsync(rowData, now, actor, result);
                                break;
                            case "ApplicantTypes":
                                await UpsertApplicantTypeAsync(rowData, now, actor, result);
                                break;
                            case "BuildingPermitCategories":
                                await UpsertBuildingPermitCategoryAsync(rowData, now, actor, result);
                                break;
                            case "Provinces":
                                await UpsertProvinceAsync(rowData, now, actor, result);
                                break;
                            case "LGUs":
                                await UpsertLguAsync(rowData, now, actor, result);
                                break;
                            case "Barangays":
                                await UpsertBarangayAsync(rowData, now, actor, result);
                                break;
                            case "RequirementClassifications":
                                await UpsertRequirementClassificationAsync(rowData, now, actor, result);
                                break;
                            case "RequirementCategories":
                                await UpsertRequirementCategoryAsync(rowData, now, actor, result);
                                break;
                            case "Requirements":
                                await UpsertRequirementAsync(rowData, now, actor, result);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new MaintenanceImportErrorDto
                        {
                            SheetName = sheet.Name ?? string.Empty,
                            RowNumber = rowNumber,
                            Message = ex.Message
                        });
                    }
                }

                // Persist each completed sheet so child sheets can resolve parents
                // created earlier in the workbook within the same transaction.
                if (result.Errors.Count == 0)
                    await _context.SaveChangesAsync();
            }

            if (result.Errors.Count > 0)
            {
                await transaction.RollbackAsync();
                return result;
            }

            await transaction.CommitAsync();
            return result;
        }

        public Task<MemoryStream> GenerateTemplateAsync()
        {
            var stream = new MemoryStream();

            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                uint sheetId = 1;

                AppendWorksheet(workbookPart, sheets, ref sheetId, "Instructions", new[]
                {
                    new[] { "Sheet", "Purpose", "Required columns" },
                    new[] { "PermitApplicationTypes", "Seed building permit application types", "PermitAppTypeDesc, IsActive" },
                    new[] { "ProjectClassifications", "Seed project classifications", "ProjectClassDesc, IsActive" },
                    new[] { "OwnershipTypes", "Seed mode of ownership values", "OwnershipTypeDesc, IsActive" },
                    new[] { "OccupancyNatures", "Seed occupancy classifications", "OccupancyNatureDesc, IsActive" },
                    new[] { "ApplicantTypes", "Seed applicant types", "ApplicantTypeDesc, IsActive" },
                    new[] { "BuildingPermitCategories", "Seed building permit categories", "CategoryName, IsActive" },
                    new[] { "Provinces", "Seed provinces", "ProvinceName, IsActive" },
                    new[] { "LGUs", "Seed municipalities or cities", "ProvinceName, LGUName, IsActive" },
                    new[] { "Barangays", "Seed barangays", "ProvinceName, LGUName, BarangayName, IsActive" },
                    new[] { "RequirementClassifications", "Seed requirement classifications", "ReqClassDesc, ApplicationTypeScope, BuildingPermitCategoryName, IsActive" },
                    new[] { "RequirementCategories", "Seed building permit requirement categories only", "ReqClassDesc, ReqCatDesc, ApplicationTypeScope, BuildingPermitCategoryName, IsActive" },
                    new[] { "Requirements", "Seed requirements", "ReqClassDesc, ReqCatDesc (optional for CertificateOfOccupancy), ReqDesc, ApplicationTypeScope, BuildingPermitCategoryName, IsActive" },
                    new[] { "ApplicationTypeScope", "Allowed values", "BuildingPermit, CertificateOfOccupancy, Both" },
                    new[] { "IsActive", "Allowed values", "TRUE or FALSE" },
                    new[] { "Notes", "Behavior", "Existing referenced values should be deactivated instead of deleted." }
                });

                AppendWorksheet(workbookPart, sheets, ref sheetId, "PermitApplicationTypes", BuildSheet(new[] { "PermitAppTypeDesc", "IsActive" }, new[] { "New Construction", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "ProjectClassifications", BuildSheet(new[] { "ProjectClassDesc", "IsActive" }, new[] { "Residential", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "OwnershipTypes", BuildSheet(new[] { "OwnershipTypeDesc", "IsActive" }, new[] { "Single Ownership", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "OccupancyNatures", BuildSheet(new[] { "OccupancyNatureDesc", "IsActive" }, new[] { "Residential Dwelling", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "ApplicantTypes", BuildSheet(new[] { "ApplicantTypeDesc", "IsActive" }, new[] { "Owner", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "BuildingPermitCategories", BuildSheet(new[] { "CategoryName", "IsActive" }, new[] { "Simple", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "Provinces", BuildSheet(new[] { "ProvinceName", "IsActive" }, new[] { "Batangas", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "LGUs", BuildSheet(new[] { "ProvinceName", "LGUName", "IsActive" }, new[] { "Batangas", "Batangas City", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "Barangays", BuildSheet(new[] { "ProvinceName", "LGUName", "BarangayName", "IsActive" }, new[] { "Batangas", "Batangas City", "Pallocan West", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "RequirementClassifications", BuildSheet(new[] { "ReqClassDesc", "ApplicationTypeScope", "BuildingPermitCategoryName", "IsActive" }, new[] { "Administrative", "BuildingPermit", "Simple", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "RequirementCategories", BuildSheet(new[] { "ReqClassDesc", "ReqCatDesc", "ApplicationTypeScope", "BuildingPermitCategoryName", "IsActive" }, new[] { "Administrative", "General Documents", "BuildingPermit", "Simple", "TRUE" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, "Requirements", BuildSheet(new[] { "ReqClassDesc", "ReqCatDesc", "ReqDesc", "ApplicationTypeScope", "BuildingPermitCategoryName", "IsActive" }, new[] { "Administrative", "General Documents", "Signed application form", "BuildingPermit", "Simple", "TRUE" }, new[] { "Certificate of Occupancy Requirements", string.Empty, "Fire Safety Inspection Certificate (FSIC) from BFP", "CertificateOfOccupancy", string.Empty, "TRUE" }));

                workbookPart.Workbook.Save();
            }

            stream.Position = 0;
            return Task.FromResult(stream);
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetPermitApplicationTypesAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.PermitApplicationTypes.IgnoreQueryFilters()
                : _context.PermitApplicationTypes.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.PermitAppTypeDesc.Contains(search));

            return await query.OrderBy(x => x.PermitAppTypeDesc).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = PermitApplicationTypes,
                Id = x.Id,
                Name = x.PermitAppTypeDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetProjectClassificationsAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.ProjectClassifications.IgnoreQueryFilters()
                : _context.ProjectClassifications.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.ProjectClassDesc.Contains(search));

            return await query.OrderBy(x => x.ProjectClassDesc).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = ProjectClassifications,
                Id = x.Id,
                Name = x.ProjectClassDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetOwnershipTypesAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.OwnershipTypes.IgnoreQueryFilters()
                : _context.OwnershipTypes.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.OwnershipTypeDesc.Contains(search));

            return await query.OrderBy(x => x.OwnershipTypeDesc).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = OwnershipTypes,
                Id = x.Id,
                Name = x.OwnershipTypeDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetOccupancyNaturesAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.OccupancyNatures.IgnoreQueryFilters()
                : _context.OccupancyNatures.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.OccupancyNatureDesc.Contains(search));

            return await query.OrderBy(x => x.OccupancyNatureDesc).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = OccupancyNatures,
                Id = x.Id,
                Name = x.OccupancyNatureDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetApplicantTypesAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.ApplicantTypes.IgnoreQueryFilters()
                : _context.ApplicantTypes.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.ApplicantTypeDesc.Contains(search));

            return await query.OrderBy(x => x.ApplicantTypeDesc).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = ApplicantTypes,
                Id = x.Id,
                Name = x.ApplicantTypeDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetBuildingPermitCategoriesAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.BuildingPermitCategories.IgnoreQueryFilters()
                : _context.BuildingPermitCategories.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.CategoryName.Contains(search) || x.Description.Contains(search));

            return await query.OrderBy(x => x.CategoryName).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = BuildingPermitCategories,
                Id = x.Id,
                Name = x.CategoryName,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetProvincesAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.Provinces.IgnoreQueryFilters()
                : _context.Provinces.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.ProvinceName.Contains(search));

            return await query.OrderBy(x => x.ProvinceName).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = Provinces,
                Id = x.Id,
                Name = x.ProvinceName,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetLgusAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.LGUs.IgnoreQueryFilters()
                : _context.LGUs.AsQueryable();

            query = query.Include(x => x.Province);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.LGUName.Contains(search) || x.Province.ProvinceName.Contains(search));

            return await query.OrderBy(x => x.Province.ProvinceName).ThenBy(x => x.LGUName).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = Lgus,
                Id = x.Id,
                Name = x.LGUName,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                ParentId = x.ProvinceId,
                ParentName = x.Province.ProvinceName
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetBarangaysAsync(bool includeInactive, bool includeDeleted, string? search)
        {
            var query = includeDeleted
                ? _context.Barangays.IgnoreQueryFilters()
                : _context.Barangays.AsQueryable();

            query = query.Include(x => x.LGU);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.BarangayName.Contains(search) || x.LGU.LGUName.Contains(search));

            return await query.OrderBy(x => x.LGU.LGUName).ThenBy(x => x.BarangayName).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = Barangays,
                Id = x.Id,
                Name = x.BarangayName,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                ParentId = x.LGUId,
                ParentName = x.LGU.LGUName
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetRequirementClassificationsAsync(bool includeInactive, bool includeDeleted, string? search, string? applicationType)
        {
            var query = includeDeleted
                ? _context.RequirementClassifications.IgnoreQueryFilters()
                : _context.RequirementClassifications.AsQueryable();

            query = query.Include(x => x.BuildingPermitCategory);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.ReqClassDesc.Contains(search));

            if (!string.IsNullOrWhiteSpace(applicationType))
                query = query.Where(x =>
                    x.ApplicationTypeScope == MaintenanceApplicationScopes.Both ||
                    x.ApplicationTypeScope == applicationType);

            return await query.OrderBy(x => x.ReqClassDesc).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = RequirementClassifications,
                Id = x.Id,
                Name = x.ReqClassDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                ApplicationTypeScope = x.ApplicationTypeScope,
                BuildingPermitCategoryId = x.BuildingPermitCategoryId,
                BuildingPermitCategoryName = x.BuildingPermitCategory != null ? x.BuildingPermitCategory.CategoryName : null
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetRequirementCategoriesAsync(bool includeInactive, bool includeDeleted, string? search, string? applicationType)
        {
            if (string.Equals(applicationType, MaintenanceApplicationScopes.CertificateOfOccupancy, StringComparison.OrdinalIgnoreCase))
                return Array.Empty<MaintenanceLookupItemDto>();

            var query = includeDeleted
                ? _context.RequirementCategorys.IgnoreQueryFilters()
                : _context.RequirementCategorys.AsQueryable();

            query = query
                .Include(x => x.RequirementClassification)
                .Include(x => x.BuildingPermitCategory);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.ReqCatDesc.Contains(search) || x.RequirementClassification.ReqClassDesc.Contains(search));

            if (!string.IsNullOrWhiteSpace(applicationType))
                query = query.Where(x =>
                    x.ApplicationTypeScope == MaintenanceApplicationScopes.Both ||
                    x.ApplicationTypeScope == applicationType);

            return await query.OrderBy(x => x.RequirementClassification.ReqClassDesc).ThenBy(x => x.ReqCatDesc).Select(x => new MaintenanceLookupItemDto
            {
                EntityType = RequirementCategories,
                Id = x.Id,
                Name = x.ReqCatDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                ApplicationTypeScope = x.ApplicationTypeScope,
                BuildingPermitCategoryId = x.BuildingPermitCategoryId,
                BuildingPermitCategoryName = x.BuildingPermitCategory != null ? x.BuildingPermitCategory.CategoryName : null,
                ParentId = x.ReqClassId,
                ParentName = x.RequirementClassification.ReqClassDesc
            }).ToListAsync();
        }

        private async Task<IEnumerable<MaintenanceLookupItemDto>> GetRequirementsAsync(bool includeInactive, bool includeDeleted, string? search, string? applicationType)
        {
            var query = includeDeleted
                ? _context.Requirements.IgnoreQueryFilters()
                : _context.Requirements.AsQueryable();

            query = query
                .Include(x => x.RequirementCategory)
                .Include(x => x.BuildingPermitCategory);

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.ReqDesc.Contains(search) || x.RequirementCategory.ReqCatDesc.Contains(search));

            if (!string.IsNullOrWhiteSpace(applicationType))
                query = query.Where(x =>
                    x.ApplicationTypeScope == MaintenanceApplicationScopes.Both ||
                    x.ApplicationTypeScope == applicationType);

            var isCertificateOfOccupancy = string.Equals(applicationType, MaintenanceApplicationScopes.CertificateOfOccupancy, StringComparison.OrdinalIgnoreCase);

            return await query
                .OrderBy(x => isCertificateOfOccupancy ? x.ReqDesc : x.RequirementCategory.ReqCatDesc)
                .ThenBy(x => x.ReqDesc)
                .Select(x => new MaintenanceLookupItemDto
            {
                EntityType = Requirements,
                Id = x.Id,
                Name = x.ReqDesc,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                ApplicationTypeScope = x.ApplicationTypeScope,
                BuildingPermitCategoryId = x.BuildingPermitCategoryId,
                BuildingPermitCategoryName = x.BuildingPermitCategory != null ? x.BuildingPermitCategory.CategoryName : null,
                ParentId = x.ReqCatId,
                ParentName = isCertificateOfOccupancy ? null : x.RequirementCategory.ReqCatDesc
            }).ToListAsync();
        }

        private async Task<MaintenanceLookupItemDto> SetProvinceStatusAsync(int id, bool isActive, DateTime now, string actor)
        {
            var province = await _context.Provinces.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Province not found.");

            ApplyStatus(province, isActive, now, actor);

            if (!isActive)
            {
                var lgus = await _context.LGUs.IgnoreQueryFilters().Where(x => x.ProvinceId == id).ToListAsync();
                var lguIds = lgus.Select(x => x.Id).ToList();
                var barangays = await _context.Barangays.IgnoreQueryFilters().Where(x => lguIds.Contains(x.LGUId)).ToListAsync();

                foreach (var lgu in lgus)
                    ApplyStatus(lgu, false, now, actor);

                foreach (var barangay in barangays)
                    ApplyStatus(barangay, false, now, actor);
            }

            return new MaintenanceLookupItemDto
            {
                EntityType = Provinces,
                Id = province.Id,
                Name = province.ProvinceName,
                IsActive = province.IsActive,
                IsDeleted = province.IsDeleted
            };
        }

        private async Task<MaintenanceLookupItemDto> SetLguStatusAsync(int id, bool isActive, DateTime now, string actor)
        {
            var lgu = await _context.LGUs.IgnoreQueryFilters().Include(x => x.Province).FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Municipality not found.");

            if (isActive && !lgu.Province.IsActive)
                throw new InvalidOperationException("Cannot activate a municipality while its province is inactive.");

            ApplyStatus(lgu, isActive, now, actor);

            if (!isActive)
            {
                var barangays = await _context.Barangays.IgnoreQueryFilters().Where(x => x.LGUId == id).ToListAsync();
                foreach (var barangay in barangays)
                    ApplyStatus(barangay, false, now, actor);
            }

            return new MaintenanceLookupItemDto
            {
                EntityType = Lgus,
                Id = lgu.Id,
                Name = lgu.LGUName,
                IsActive = lgu.IsActive,
                IsDeleted = lgu.IsDeleted,
                ParentId = lgu.ProvinceId,
                ParentName = lgu.Province.ProvinceName
            };
        }

        private async Task<MaintenanceLookupItemDto> SetBarangayStatusAsync(int id, bool isActive, DateTime now, string actor)
        {
            var barangay = await _context.Barangays.IgnoreQueryFilters().Include(x => x.LGU).FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Barangay not found.");

            if (isActive && !barangay.LGU.IsActive)
                throw new InvalidOperationException("Cannot activate a barangay while its municipality is inactive.");

            ApplyStatus(barangay, isActive, now, actor);

            return new MaintenanceLookupItemDto
            {
                EntityType = Barangays,
                Id = barangay.Id,
                Name = barangay.BarangayName,
                IsActive = barangay.IsActive,
                IsDeleted = barangay.IsDeleted,
                ParentId = barangay.LGUId,
                ParentName = barangay.LGU.LGUName
            };
        }

        private async Task<MaintenanceLookupItemDto> SetRequirementClassificationStatusAsync(int id, bool isActive, DateTime now, string actor)
        {
            var classification = await _context.RequirementClassifications.IgnoreQueryFilters()
                .Include(x => x.BuildingPermitCategory)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Requirement classification not found.");

            ApplyStatus(classification, isActive, now, actor);

            if (!isActive)
            {
                var categories = await _context.RequirementCategorys.IgnoreQueryFilters().Where(x => x.ReqClassId == id).ToListAsync();
                var categoryIds = categories.Select(x => x.Id).ToList();
                var requirements = await _context.Requirements.IgnoreQueryFilters().Where(x => categoryIds.Contains(x.ReqCatId)).ToListAsync();

                foreach (var category in categories)
                    ApplyStatus(category, false, now, actor);

                foreach (var requirement in requirements)
                    ApplyStatus(requirement, false, now, actor);
            }

            return new MaintenanceLookupItemDto
            {
                EntityType = RequirementClassifications,
                Id = classification.Id,
                Name = classification.ReqClassDesc,
                IsActive = classification.IsActive,
                IsDeleted = classification.IsDeleted,
                ApplicationTypeScope = classification.ApplicationTypeScope,
                BuildingPermitCategoryId = classification.BuildingPermitCategoryId,
                BuildingPermitCategoryName = classification.BuildingPermitCategory != null ? classification.BuildingPermitCategory.CategoryName : null
            };
        }

        private async Task<MaintenanceLookupItemDto> SetRequirementCategoryStatusAsync(int id, bool isActive, DateTime now, string actor)
        {
            var category = await _context.RequirementCategorys.IgnoreQueryFilters()
                .Include(x => x.RequirementClassification)
                .Include(x => x.BuildingPermitCategory)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Requirement category not found.");

            if (isActive && !category.RequirementClassification.IsActive)
                throw new InvalidOperationException("Cannot activate a requirement category while its parent classification is inactive.");

            ApplyStatus(category, isActive, now, actor);

            if (!isActive)
            {
                var requirements = await _context.Requirements.IgnoreQueryFilters().Where(x => x.ReqCatId == id).ToListAsync();
                foreach (var requirement in requirements)
                    ApplyStatus(requirement, false, now, actor);
            }

            return new MaintenanceLookupItemDto
            {
                EntityType = RequirementCategories,
                Id = category.Id,
                Name = category.ReqCatDesc,
                IsActive = category.IsActive,
                IsDeleted = category.IsDeleted,
                ApplicationTypeScope = category.ApplicationTypeScope,
                BuildingPermitCategoryId = category.BuildingPermitCategoryId,
                BuildingPermitCategoryName = category.BuildingPermitCategory != null ? category.BuildingPermitCategory.CategoryName : null,
                ParentId = category.ReqClassId,
                ParentName = category.RequirementClassification.ReqClassDesc
            };
        }

        private async Task<MaintenanceLookupItemDto> SetRequirementStatusAsync(int id, bool isActive, DateTime now, string actor)
        {
            var requirement = await _context.Requirements.IgnoreQueryFilters()
                .Include(x => x.RequirementCategory)
                .Include(x => x.BuildingPermitCategory)
                .FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Requirement not found.");

            if (isActive && !requirement.RequirementCategory.IsActive)
                throw new InvalidOperationException("Cannot activate a requirement while its parent category is inactive.");

            ApplyStatus(requirement, isActive, now, actor);

            return new MaintenanceLookupItemDto
            {
                EntityType = Requirements,
                Id = requirement.Id,
                Name = requirement.ReqDesc,
                IsActive = requirement.IsActive,
                IsDeleted = requirement.IsDeleted,
                ApplicationTypeScope = requirement.ApplicationTypeScope,
                BuildingPermitCategoryId = requirement.BuildingPermitCategoryId,
                BuildingPermitCategoryName = requirement.BuildingPermitCategory != null ? requirement.BuildingPermitCategory.CategoryName : null,
                ParentId = requirement.ReqCatId,
                ParentName = requirement.RequirementCategory.ReqCatDesc
            };
        }

        private async Task<MaintenanceLookupItemDto> SetSimpleStatusAsync<TEntity>(
            IQueryable<TEntity> query,
            int id,
            Func<TEntity, string> nameSelector,
            string entityType,
            bool isActive,
            DateTime now,
            string actor)
            where TEntity : class
        {
            var entity = await query.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id)
                ?? throw new InvalidOperationException("Lookup item not found.");

            SetProperty(entity, "IsActive", isActive);
            SetProperty(entity, "UpdatedAt", now);
            SetProperty(entity, "UpdatedBy", actor);

            return new MaintenanceLookupItemDto
            {
                EntityType = entityType,
                Id = id,
                Name = nameSelector(entity),
                IsActive = isActive,
                IsDeleted = (bool)(typeof(TEntity).GetProperty("IsDeleted")?.GetValue(entity) ?? false)
            };
        }

        private static void ApplyStatus(object entity, bool isActive, DateTime now, string actor)
        {
            SetProperty(entity, "IsActive", isActive);
            SetProperty(entity, "UpdatedAt", now);
            SetProperty(entity, "UpdatedBy", actor);
        }

        private static void SetProperty(object entity, string propertyName, object? value)
        {
            entity.GetType().GetProperty(propertyName)?.SetValue(entity, value);
        }

        private async Task<MaintenanceReferenceUsageItemDto> BuildUsageItemAsync(string source, Task<int> countTask)
        {
            return new MaintenanceReferenceUsageItemDto
            {
                Source = source,
                Count = await countTask
            };
        }

        private async Task<MaintenanceReferenceUsageDto> BuildUsageAsync(
            string entityType,
            int id,
            string? name,
            IEnumerable<MaintenanceReferenceUsageItemDto> usages)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Lookup item not found.");

            var usageList = usages.Where(x => x.Count > 0).ToList();
            var totalReferences = usageList.Sum(x => x.Count);

            var activeApplicationReferences = entityType switch
            {
                PermitApplicationTypes => await CountActiveBuildingPermitsAsync(x => x.PermitAppTypeId == id),
                ProjectClassifications => await CountActiveBuildingPermitsAsync(x => x.ProjectClassId == id),
                OwnershipTypes => await CountActiveBuildingPermitAppInfosAsync(x => x.OwnershipTypeId == id),
                OccupancyNatures => await CountActiveBuildingPermitsAsync(x => x.OccupancyNatureId == id)
                    + await CountActiveCoOAppsAsync(x => x.OccupancyNatureId == id),
                ApplicantTypes => await CountActiveBuildingPermitAppInfosAsync(x => x.ApplicantTypeId == id)
                    + await CountActiveCoOAppsAsync(x => x.ApplicantTypeId == id),
                Provinces => await CountActiveBuildingPermitsAsync(x => x.ProvinceId == id)
                    + await CountActiveCoOAppsAsync(x => x.ProvinceId == id),
                Lgus => await CountActiveBuildingPermitsAsync(x => x.LGUId == id)
                    + await CountActiveCoOAppsAsync(x => x.LGUId == id),
                Barangays => await CountActiveBuildingPermitsAsync(x => x.BarangayId == id)
                    + await CountActiveCoOAppsAsync(x => x.BarangayId == id),
                _ => 0
            };

            return new MaintenanceReferenceUsageDto
            {
                EntityType = entityType,
                Id = id,
                Name = name,
                TotalReferences = totalReferences,
                ActiveApplicationReferences = activeApplicationReferences,
                Usages = usageList
            };
        }

        private async Task<int> CountActiveBuildingPermitsAsync(Func<BuildingPermit, bool> predicate)
        {
            var buildingPermitIds = _context.BuildingPermits
                .IgnoreQueryFilters()
                .AsEnumerable()
                .Where(predicate)
                .Select(x => x.ApplicationId)
                .ToList();

            return await CountActiveApplicationsAsync(buildingPermitIds);
        }

        private async Task<int> CountActiveBuildingPermitAppInfosAsync(Func<BuildingPermitAppInfo, bool> predicate)
        {
            var buildingPermitIds = _context.BuildingPermitAppInfos
                .IgnoreQueryFilters()
                .Include(x => x.BuildingPermit)
                .AsEnumerable()
                .Where(predicate)
                .Select(x => x.BuildingPermit.ApplicationId)
                .ToList();

            return await CountActiveApplicationsAsync(buildingPermitIds);
        }

        private async Task<int> CountActiveCoOAppsAsync(Func<CoOApp, bool> predicate)
        {
            var applicationIds = _context.CoOApps
                .IgnoreQueryFilters()
                .AsEnumerable()
                .Where(predicate)
                .Select(x => x.ApplicationId)
                .ToList();

            return await CountActiveApplicationsAsync(applicationIds);
        }

        private Task<int> CountActiveApplicationsAsync(IEnumerable<int> applicationIds)
        {
            var ids = applicationIds.Distinct().ToList();
            if (ids.Count == 0)
                return Task.FromResult(0);

            return _context.Applications
                .IgnoreQueryFilters()
                .CountAsync(x => ids.Contains(x.Id)
                    && x.Status != ApplicationWorkflowDefinitions.OverallStatuses.ApprovedForIssuance
                    && !ApplicationWorkflowDefinitions.IsTerminalStatus(x.Status));
        }

        private async Task<object> CreateSimpleItemAsync(
            Task<bool> existsTask,
            Action addAction,
            Func<Task<object>> selector,
            string label)
        {
            if (await existsTask)
                throw new InvalidOperationException($"{label} already exists.");

            addAction();
            await _context.SaveChangesAsync();
            return await selector();
        }

        private async Task<object> CreateLguAsync(string name, int? parentId)
        {
            if (!parentId.HasValue)
                throw new InvalidOperationException("Province is required.");

            var exists = await _context.LGUs.IgnoreQueryFilters()
                .AnyAsync(x => x.ProvinceId == parentId.Value && x.LGUName == name);
            if (exists)
                throw new InvalidOperationException("Municipality / City already exists.");

            _context.LGUs.Add(new LGU
            {
                ProvinceId = parentId.Value,
                LGUName = name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserName ?? "System"
            });

            await _context.SaveChangesAsync();

            return await _context.LGUs.IgnoreQueryFilters()
                .Where(x => x.ProvinceId == parentId.Value && x.LGUName == name)
                .Select(x => new { x.Id, Name = x.LGUName, x.IsActive, ParentId = x.ProvinceId })
                .FirstAsync();
        }

        private async Task<object> CreateBarangayAsync(string name, int? parentId)
        {
            if (!parentId.HasValue)
                throw new InvalidOperationException("Municipality / City is required.");

            var exists = await _context.Barangays.IgnoreQueryFilters()
                .AnyAsync(x => x.LGUId == parentId.Value && x.BarangayName == name);
            if (exists)
                throw new InvalidOperationException("Barangay already exists.");

            _context.Barangays.Add(new Barangay
            {
                LGUId = parentId.Value,
                BarangayName = name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserName ?? "System"
            });

            await _context.SaveChangesAsync();

            return await _context.Barangays.IgnoreQueryFilters()
                .Where(x => x.LGUId == parentId.Value && x.BarangayName == name)
                .Select(x => new { x.Id, Name = x.BarangayName, x.IsActive, ParentId = x.LGUId })
                .FirstAsync();
        }

        private async Task<object> CreateRequirementClassificationAsync(string name, string scope, int? buildingPermitCategoryId)
        {
            var exists = await _context.RequirementClassifications.IgnoreQueryFilters()
                .AnyAsync(x => x.ReqClassDesc == name && x.BuildingPermitCategoryId == buildingPermitCategoryId);
            if (exists)
                throw new InvalidOperationException("Requirement classification already exists.");

            if (buildingPermitCategoryId.HasValue)
            {
                if (!string.Equals(scope, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Building permit category can only be assigned to Building Permit classifications.");

                var categoryExists = await _context.BuildingPermitCategories.IgnoreQueryFilters().AnyAsync(x => x.Id == buildingPermitCategoryId.Value);
                if (!categoryExists)
                    throw new InvalidOperationException("Building permit category not found.");
            }

            _context.RequirementClassifications.Add(new RequirementClassification
            {
                ReqClassDesc = name,
                ApplicationTypeScope = scope,
                BuildingPermitCategoryId = buildingPermitCategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserName ?? "System"
            });

            await _context.SaveChangesAsync();

            return await _context.RequirementClassifications.IgnoreQueryFilters()
                .Where(x => x.ReqClassDesc == name && x.BuildingPermitCategoryId == buildingPermitCategoryId)
                .Select(x => new { x.Id, Name = x.ReqClassDesc, x.IsActive, x.ApplicationTypeScope, x.BuildingPermitCategoryId })
                .FirstAsync();
        }

        private async Task<object> CreateRequirementCategoryAsync(string name, int? parentId, string scope, int? buildingPermitCategoryId)
        {
            if (!parentId.HasValue)
                throw new InvalidOperationException("Requirement classification is required.");

            if (!string.Equals(scope, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Requirement categories are only supported for Building Permit.");

            var exists = await _context.RequirementCategorys.IgnoreQueryFilters()
                .AnyAsync(x => x.ReqClassId == parentId.Value && x.ReqCatDesc == name && x.BuildingPermitCategoryId == buildingPermitCategoryId);
            if (exists)
                throw new InvalidOperationException("Requirement category already exists.");

            if (buildingPermitCategoryId.HasValue)
            {
                if (!string.Equals(scope, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Building permit category can only be assigned to Building Permit categories.");

                var categoryExists = await _context.BuildingPermitCategories.IgnoreQueryFilters().AnyAsync(x => x.Id == buildingPermitCategoryId.Value);
                if (!categoryExists)
                    throw new InvalidOperationException("Building permit category not found.");
            }

            _context.RequirementCategorys.Add(new RequirementCategory
            {
                ReqClassId = parentId.Value,
                ReqCatDesc = name,
                ApplicationTypeScope = scope,
                BuildingPermitCategoryId = buildingPermitCategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserName ?? "System"
            });

            await _context.SaveChangesAsync();

            return await _context.RequirementCategorys.IgnoreQueryFilters()
                .Where(x => x.ReqClassId == parentId.Value && x.ReqCatDesc == name && x.BuildingPermitCategoryId == buildingPermitCategoryId)
                .Select(x => new { x.Id, Name = x.ReqCatDesc, x.IsActive, ParentId = x.ReqClassId, x.ApplicationTypeScope, x.BuildingPermitCategoryId })
                .FirstAsync();
        }

        private async Task<object> CreateRequirementAsync(string name, int? parentId, string scope, int? buildingPermitCategoryId)
        {
            if (!parentId.HasValue && !string.Equals(scope, MaintenanceApplicationScopes.CertificateOfOccupancy, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Requirement category is required.");

            var resolvedParentId = parentId;

            if (string.Equals(scope, MaintenanceApplicationScopes.CertificateOfOccupancy, StringComparison.OrdinalIgnoreCase))
            {
                resolvedParentId = await ResolveCertificateOfOccupancyGeneralCategoryIdAsync();
                buildingPermitCategoryId = null;
            }

            if (buildingPermitCategoryId.HasValue && !string.Equals(scope, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Building permit category can only be assigned to Building Permit requirements.");

            if (buildingPermitCategoryId.HasValue)
            {
                var categoryExists = await _context.BuildingPermitCategories.IgnoreQueryFilters()
                    .AnyAsync(x => x.Id == buildingPermitCategoryId.Value);
                if (!categoryExists)
                    throw new InvalidOperationException("Building permit category not found.");
            }

            var exists = await _context.Requirements.IgnoreQueryFilters()
                .AnyAsync(x => x.ReqCatId == resolvedParentId!.Value && x.ReqDesc == name && x.BuildingPermitCategoryId == buildingPermitCategoryId);
            if (exists)
                throw new InvalidOperationException("Requirement already exists.");

            _context.Requirements.Add(new Requirement
            {
                ReqCatId = resolvedParentId.Value,
                ReqDesc = name,
                ApplicationTypeScope = scope,
                BuildingPermitCategoryId = buildingPermitCategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserName ?? "System"
            });

            await _context.SaveChangesAsync();

            return await _context.Requirements.IgnoreQueryFilters()
                .Where(x => x.ReqCatId == resolvedParentId.Value && x.ReqDesc == name && x.BuildingPermitCategoryId == buildingPermitCategoryId)
                .Select(x => new { x.Id, Name = x.ReqDesc, x.IsActive, ParentId = x.ReqCatId, x.ApplicationTypeScope, x.BuildingPermitCategoryId })
                .FirstAsync();
        }

        private static string RequiredString(IDictionary<string, object?> values, string key)
        {
            if (!values.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue?.ToString()))
                throw new InvalidOperationException($"{key} is required.");

            return rawValue!.ToString()!.Trim();
        }

        private static int? OptionalInt(IDictionary<string, object?> values, string key)
        {
            if (!values.TryGetValue(key, out var rawValue) || rawValue == null)
                return null;

            return rawValue switch
            {
                int intValue => intValue,
                long longValue => checked((int)longValue),
                string stringValue when int.TryParse(stringValue, out var parsed) => parsed,
                _ => null
            };
        }

        private static string NormalizeOptionalScope(IDictionary<string, object?> values, string key)
        {
            if (!values.TryGetValue(key, out var rawValue) || string.IsNullOrWhiteSpace(rawValue?.ToString()))
                return MaintenanceApplicationScopes.Both;

            var normalized = MaintenanceApplicationScopes.Normalize(rawValue.ToString());
            if (!MaintenanceApplicationScopes.IsValid(normalized))
                throw new InvalidOperationException("ApplicationTypeScope must be BuildingPermit, CertificateOfOccupancy, or Both.");

            return normalized;
        }

        private async Task<BuildingPermitCategory?> ResolveBuildingPermitCategoryByNameAsync(string? categoryName, string scope)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return null;

            if (!string.Equals(scope, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("BuildingPermitCategoryName can only be used for Building Permit requirements.");

            return await _context.BuildingPermitCategories.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.CategoryName == categoryName.Trim())
                ?? throw new InvalidOperationException($"Building permit category '{categoryName}' does not exist.");
        }

        private async Task<int> ResolveCertificateOfOccupancyGeneralCategoryIdAsync()
        {
            var categoryId = await _context.RequirementCategorys.IgnoreQueryFilters()
                .Where(x =>
                    x.ReqCatDesc == CertificateOfOccupancyGeneralCategory
                    && x.ApplicationTypeScope == MaintenanceApplicationScopes.CertificateOfOccupancy
                    && x.RequirementClassification.ReqClassDesc == CertificateOfOccupancyRequirementsClassification)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();

            return categoryId ?? throw new InvalidOperationException("Certificate of Occupancy general requirements category is not configured.");
        }

        private async Task UpsertPermitApplicationTypeAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "PermitAppTypeDesc");
            var entity = await _context.PermitApplicationTypes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.PermitAppTypeDesc == name);

            if (entity == null)
            {
                _context.PermitApplicationTypes.Add(new PermitApplicationType { PermitAppTypeDesc = name, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.PermitAppTypeDesc = name;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertProjectClassificationAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "ProjectClassDesc");
            var entity = await _context.ProjectClassifications.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProjectClassDesc == name);

            if (entity == null)
            {
                _context.ProjectClassifications.Add(new ProjectClassification { ProjectClassDesc = name, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.ProjectClassDesc = name;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertOwnershipTypeAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "OwnershipTypeDesc");
            var entity = await _context.OwnershipTypes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.OwnershipTypeDesc == name);

            if (entity == null)
            {
                _context.OwnershipTypes.Add(new OwnershipType { OwnershipTypeDesc = name, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.OwnershipTypeDesc = name;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertOccupancyNatureAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "OccupancyNatureDesc");
            var entity = await _context.OccupancyNatures.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.OccupancyNatureDesc == name);

            if (entity == null)
            {
                _context.OccupancyNatures.Add(new OccupancyNature { OccupancyNatureDesc = name, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.OccupancyNatureDesc = name;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertApplicantTypeAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "ApplicantTypeDesc");
            var entity = await _context.ApplicantTypes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ApplicantTypeDesc == name);

            if (entity == null)
            {
                _context.ApplicantTypes.Add(new ApplicantType { ApplicantTypeDesc = name, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.ApplicantTypeDesc = name;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertBuildingPermitCategoryAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "CategoryName");
            var entity = await _context.BuildingPermitCategories.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.CategoryName == name);

            if (entity == null)
            {
                _context.BuildingPermitCategories.Add(new BuildingPermitCategory
                {
                    CategoryName = name,
                    Description = string.Empty,
                    IsActive = ParseBoolean(row, "IsActive"),
                    CreatedAt = now,
                    CreatedBy = actor
                });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.CategoryName = name;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertProvinceAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "ProvinceName");
            var entity = await _context.Provinces.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceName == name);

            if (entity == null)
            {
                _context.Provinces.Add(new Province { ProvinceName = name, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.ProvinceName = name;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertLguAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var provinceName = RequiredValue(row, "ProvinceName");
            var lguName = RequiredValue(row, "LGUName");
            var province = await _context.Provinces.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceName == provinceName)
                ?? throw new InvalidOperationException($"Province '{provinceName}' does not exist.");
            var entity = await _context.LGUs.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceId == province.Id && x.LGUName == lguName);

            if (entity == null)
            {
                _context.LGUs.Add(new LGU { ProvinceId = province.Id, LGUName = lguName, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.ProvinceId = province.Id;
            entity.LGUName = lguName;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertBarangayAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var provinceName = RequiredValue(row, "ProvinceName");
            var lguName = RequiredValue(row, "LGUName");
            var barangayName = RequiredValue(row, "BarangayName");
            var province = await _context.Provinces.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceName == provinceName)
                ?? throw new InvalidOperationException($"Province '{provinceName}' does not exist.");
            var lgu = await _context.LGUs.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceId == province.Id && x.LGUName == lguName)
                ?? throw new InvalidOperationException($"Municipality '{lguName}' does not exist in province '{provinceName}'.");
            var entity = await _context.Barangays.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.LGUId == lgu.Id && x.BarangayName == barangayName);

            if (entity == null)
            {
                _context.Barangays.Add(new Barangay { LGUId = lgu.Id, BarangayName = barangayName, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.LGUId = lgu.Id;
            entity.BarangayName = barangayName;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertRequirementClassificationAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var name = RequiredValue(row, "ReqClassDesc");
            var scope = NormalizeScope(row);
            row.TryGetValue("BuildingPermitCategoryName", out var buildingPermitCategoryName);
            var buildingPermitCategory = await ResolveBuildingPermitCategoryByNameAsync(buildingPermitCategoryName, scope);
            var buildingPermitCategoryId = buildingPermitCategory?.Id;
            var entity = await _context.RequirementClassifications.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ReqClassDesc == name && x.BuildingPermitCategoryId == buildingPermitCategoryId);

            if (entity == null)
            {
                _context.RequirementClassifications.Add(new RequirementClassification { ReqClassDesc = name, ApplicationTypeScope = scope, BuildingPermitCategoryId = buildingPermitCategoryId, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.ReqClassDesc = name;
            entity.ApplicationTypeScope = scope;
            entity.BuildingPermitCategoryId = buildingPermitCategoryId;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertRequirementCategoryAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var className = RequiredValue(row, "ReqClassDesc");
            var categoryName = RequiredValue(row, "ReqCatDesc");
            var scope = NormalizeScope(row);
            if (!string.Equals(scope, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Requirement categories are only supported for Building Permit.");
            row.TryGetValue("BuildingPermitCategoryName", out var buildingPermitCategoryName);
            var classification = await _context.RequirementClassifications.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ReqClassDesc == className)
                ?? throw new InvalidOperationException($"Requirement classification '{className}' does not exist.");
            var buildingPermitCategory = await ResolveBuildingPermitCategoryByNameAsync(buildingPermitCategoryName, scope);
            var buildingPermitCategoryId = buildingPermitCategory?.Id;
            var entity = await _context.RequirementCategorys.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ReqClassId == classification.Id && x.ReqCatDesc == categoryName && x.BuildingPermitCategoryId == buildingPermitCategoryId);

            if (entity == null)
            {
                _context.RequirementCategorys.Add(new RequirementCategory { ReqClassId = classification.Id, ReqCatDesc = categoryName, ApplicationTypeScope = scope, BuildingPermitCategoryId = buildingPermitCategoryId, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.ReqClassId = classification.Id;
            entity.ReqCatDesc = categoryName;
            entity.ApplicationTypeScope = scope;
            entity.BuildingPermitCategoryId = buildingPermitCategoryId;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private async Task UpsertRequirementAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var className = RequiredValue(row, "ReqClassDesc");
            var reqName = RequiredValue(row, "ReqDesc");
            var scope = NormalizeScope(row);
            row.TryGetValue("BuildingPermitCategoryName", out var buildingPermitCategoryName);
            var classification = await _context.RequirementClassifications.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ReqClassDesc == className)
                ?? throw new InvalidOperationException($"Requirement classification '{className}' does not exist.");
            RequirementCategory category;
            if (string.Equals(scope, MaintenanceApplicationScopes.CertificateOfOccupancy, StringComparison.OrdinalIgnoreCase))
            {
                var categoryId = await ResolveCertificateOfOccupancyGeneralCategoryIdAsync();
                category = await _context.RequirementCategorys.IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.Id == categoryId)
                    ?? throw new InvalidOperationException("Certificate of Occupancy general requirements category is not configured.");
                buildingPermitCategoryName = null;
            }
            else
            {
                var categoryName = RequiredValue(row, "ReqCatDesc");
                category = await _context.RequirementCategorys.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ReqClassId == classification.Id && x.ReqCatDesc == categoryName)
                    ?? throw new InvalidOperationException($"Requirement category '{categoryName}' does not exist in classification '{className}'.");
            }
            var buildingPermitCategory = await ResolveBuildingPermitCategoryByNameAsync(buildingPermitCategoryName, scope);
            var buildingPermitCategoryId = buildingPermitCategory?.Id;
            var entity = await _context.Requirements.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ReqCatId == category.Id && x.ReqDesc == reqName && x.BuildingPermitCategoryId == buildingPermitCategoryId);

            if (entity == null)
            {
                _context.Requirements.Add(new Requirement { ReqCatId = category.Id, ReqDesc = reqName, ApplicationTypeScope = scope, BuildingPermitCategoryId = buildingPermitCategoryId, IsActive = ParseBoolean(row, "IsActive"), CreatedAt = now, CreatedBy = actor });
                result.CreatedCount++;
                return;
            }

            UpdateImportAudit(entity, now, actor, result);
            entity.ReqCatId = category.Id;
            entity.ReqDesc = reqName;
            entity.ApplicationTypeScope = scope;
            entity.BuildingPermitCategoryId = buildingPermitCategoryId;
            entity.IsActive = ParseBoolean(row, "IsActive");
        }

        private static void UpdateImportAudit(object entity, DateTime now, string actor, MaintenanceImportResultDto result)
        {
            var wasDeleted = (bool)(entity.GetType().GetProperty("IsDeleted")?.GetValue(entity) ?? false);
            if (wasDeleted)
            {
                SetProperty(entity, "IsDeleted", false);
                result.ReactivatedCount++;
            }
            else
            {
                result.UpdatedCount++;
            }

            SetProperty(entity, "UpdatedAt", now);
            SetProperty(entity, "UpdatedBy", actor);
        }

        private static string RequiredValue(Dictionary<string, string> row, string key)
        {
            if (!row.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"'{key}' is required.");

            return value.Trim();
        }

        private static bool ParseBoolean(Dictionary<string, string> row, string key)
        {
            if (!row.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                return true;

            if (bool.TryParse(value, out var parsed))
                return parsed;

            if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase))
                return false;

            throw new InvalidOperationException($"'{key}' must be TRUE or FALSE.");
        }

        private static string NormalizeScope(Dictionary<string, string> row)
        {
            row.TryGetValue("ApplicationTypeScope", out var scope);
            var normalizedScope = MaintenanceApplicationScopes.Normalize(scope);
            if (!MaintenanceApplicationScopes.IsValid(normalizedScope))
                throw new InvalidOperationException("ApplicationTypeScope must be BuildingPermit, CertificateOfOccupancy, or Both.");

            return normalizedScope;
        }

        private static string NormalizeEntityType(string entityType)
        {
            return entityType.Trim().ToLowerInvariant() switch
            {
                "permitapplicationtypes" or "permit-application-types" => PermitApplicationTypes,
                "projectclassifications" or "project-classifications" => ProjectClassifications,
                "ownershiptypes" or "ownership-types" => OwnershipTypes,
                "occupancynatures" or "occupancy-natures" => OccupancyNatures,
                "applicanttypes" or "applicant-types" => ApplicantTypes,
                "buildingpermitcategories" or "building-permit-categories" => BuildingPermitCategories,
                "provinces" => Provinces,
                "lgus" or "municipalities" => Lgus,
                "barangays" => Barangays,
                "requirementclassifications" or "requirement-classifications" => RequirementClassifications,
                "requirementcategories" or "requirement-categories" => RequirementCategories,
                "requirements" => Requirements,
                _ => throw new InvalidOperationException($"Unsupported maintenance entity type '{entityType}'.")
            };
        }

        private static string? NormalizeOptionalApplicationType(string? applicationType)
        {
            if (string.IsNullOrWhiteSpace(applicationType))
                return null;

            var normalized = MaintenanceApplicationScopes.Normalize(applicationType);
            if (!MaintenanceApplicationScopes.IsValid(normalized))
                throw new InvalidOperationException("applicationType must be BuildingPermit or CertificateOfOccupancy.");

            return normalized == MaintenanceApplicationScopes.Both ? null : normalized;
        }

        private static IEnumerable<string[]> BuildSheet(string[] header, params string[][] samples)
        {
            return new[] { header }.Concat(samples);
        }

        private static void AppendWorksheet(WorkbookPart workbookPart, Sheets sheets, ref uint sheetId, string sheetName, IEnumerable<string[]> rows)
        {
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();

            foreach (var rowValues in rows)
            {
                var row = new Row();
                foreach (var value in rowValues)
                {
                    row.Append(new Cell
                    {
                        DataType = CellValues.InlineString,
                        InlineString = new InlineString(new Text(value ?? string.Empty))
                    });
                }

                sheetData.Append(row);
            }

            worksheetPart.Worksheet = new Worksheet(sheetData);
            sheets.Append(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = sheetId++,
                Name = sheetName
            });
        }

        private static IEnumerable<List<string>> ReadWorksheetRows(WorkbookPart workbookPart, WorksheetPart worksheetPart)
        {
            var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>()?.Elements<Row>() ?? Enumerable.Empty<Row>();
            foreach (var row in rows)
                yield return row.Elements<Cell>().Select(cell => ReadCellValue(workbookPart, cell)).ToList();
        }

        private static string ReadCellValue(WorkbookPart workbookPart, Cell cell)
        {
            var value = cell.CellValue?.InnerText ?? cell.InnerText ?? string.Empty;
            if (cell.DataType == null)
                return value;

            if (cell.DataType.Value == CellValues.SharedString)
                return workbookPart.SharedStringTablePart?.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText ?? string.Empty;

            if (cell.DataType.Value == CellValues.InlineString)
                return cell.InlineString?.Text?.Text ?? cell.InnerText ?? string.Empty;

            if (cell.DataType.Value == CellValues.Boolean)
                return value == "1" ? "TRUE" : "FALSE";

            return value;
        }

        private static Dictionary<string, string> MapRow(IReadOnlyList<string> headers, IReadOnlyList<string> values)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 0; index < headers.Count; index++)
            {
                var key = headers[index]?.Trim();
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                result[key] = index < values.Count ? values[index]?.Trim() ?? string.Empty : string.Empty;
            }

            return result;
        }
    }
}
