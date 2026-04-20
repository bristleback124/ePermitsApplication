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

        private const string PermitApplicationTypesSheet = "PermitApplicationTypes";
        private const string ProjectClassificationsSheet = "ProjectClassifications";
        private const string OwnershipTypesSheet = "OwnershipTypes";
        private const string OccupancyNaturesSheet = "OccupancyNatures";
        private const string ApplicantTypesSheet = "ApplicantTypes";
        private const string ProvincesSheet = "Provinces";
        private const string LgusSheet = "LGUs";
        private const string BarangaysSheet = "Barangays";
        private const string RequirementCategoriesSheet = "BuildingPermitReqCategories";
        private const string BuildingPermitRequirementsSheet = "BuildingPermitRequirements";
        private const string CertificateOfOccupancyRequirementsSheet = "CertificateOfOccupancyRequirements";

        private static readonly IReadOnlyList<(string SheetName, string GroupLabel)> ImportSheetOrder = new[]
        {
            (PermitApplicationTypesSheet, "Types of Application"),
            (ProjectClassificationsSheet, "Project Classification"),
            (OwnershipTypesSheet, "Mode of Ownership"),
            (OccupancyNaturesSheet, "Occupancy Classification"),
            (ApplicantTypesSheet, "Applicant Type"),
            (ProvincesSheet, "Province"),
            (LgusSheet, "Municipality / City"),
            (BarangaysSheet, "Barangay"),
            (RequirementCategoriesSheet, "Building Permit Category"),
            (BuildingPermitRequirementsSheet, "Requirements (Building Permit)"),
            (CertificateOfOccupancyRequirementsSheet, "Requirements (Certificate of Occupancy)"),
        };

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

            var sheetByName = workbookPart.Workbook.Sheets!
                .Elements<Sheet>()
                .Where(s => s.Name?.Value != null)
                .GroupBy(s => s.Name!.Value!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var (sheetName, groupLabel) in ImportSheetOrder)
            {
                var summary = new MaintenanceImportGroupSummaryDto
                {
                    SheetName = sheetName,
                    GroupLabel = groupLabel,
                };
                result.Groups.Add(summary);

                if (!sheetByName.TryGetValue(sheetName, out var sheet))
                    continue;

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
                        switch (sheetName)
                        {
                            case PermitApplicationTypesSheet:
                                await InsertPermitApplicationTypeIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case ProjectClassificationsSheet:
                                await InsertProjectClassificationIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case OwnershipTypesSheet:
                                await InsertOwnershipTypeIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case OccupancyNaturesSheet:
                                await InsertOccupancyNatureIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case ApplicantTypesSheet:
                                await InsertApplicantTypeIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case ProvincesSheet:
                                await InsertProvinceIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case LgusSheet:
                                await InsertLguIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case BarangaysSheet:
                                await InsertBarangayIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case RequirementCategoriesSheet:
                                await InsertRequirementCategoryIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case BuildingPermitRequirementsSheet:
                                await InsertBuildingPermitRequirementIfMissingAsync(rowData, now, actor, summary);
                                break;
                            case CertificateOfOccupancyRequirementsSheet:
                                await InsertCertificateOfOccupancyRequirementIfMissingAsync(rowData, now, actor, summary);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new MaintenanceImportErrorDto
                        {
                            SheetName = sheetName,
                            RowNumber = rowNumber,
                            Message = ex.Message
                        });
                    }
                }

                result.CreatedCount += summary.CreatedCount;
                result.SkippedCount += summary.SkippedCount;

                // Flush after each sheet so later sheets can resolve parents created
                // earlier in the same workbook within this transaction.
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
                    new[] { PermitApplicationTypesSheet, "Types of Application", "PermitAppTypeDesc" },
                    new[] { ProjectClassificationsSheet, "Project Classification", "ProjectClassDesc" },
                    new[] { OwnershipTypesSheet, "Mode of Ownership", "OwnershipTypeDesc" },
                    new[] { OccupancyNaturesSheet, "Occupancy Classification", "OccupancyNatureDesc" },
                    new[] { ApplicantTypesSheet, "Applicant Type", "ApplicantTypeDesc" },
                    new[] { ProvincesSheet, "Province", "ProvinceName" },
                    new[] { LgusSheet, "Municipality / City", "ProvinceName, LGUName" },
                    new[] { BarangaysSheet, "Barangay", "ProvinceName, LGUName, BarangayName" },
                    new[] { RequirementCategoriesSheet, "Building Permit Category (requirement grouping)", "BuildingPermitReqCatDesc, BuildingPermitCategoryName" },
                    new[] { BuildingPermitRequirementsSheet, "Requirements scoped to Building Permit", "BuildingPermitReqCatDesc, ReqDesc, BuildingPermitCategoryName" },
                    new[] { CertificateOfOccupancyRequirementsSheet, "Requirements scoped to Certificate of Occupancy", "ReqDesc" },
                    new[] { "Notes", "Behavior", "Existing values are skipped. Rows are only added when they do not already exist for that group." },
                });

                AppendWorksheet(workbookPart, sheets, ref sheetId, PermitApplicationTypesSheet, BuildSheet(new[] { "PermitAppTypeDesc" }, new[] { "New Construction" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, ProjectClassificationsSheet, BuildSheet(new[] { "ProjectClassDesc" }, new[] { "Residential" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, OwnershipTypesSheet, BuildSheet(new[] { "OwnershipTypeDesc" }, new[] { "Single Ownership" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, OccupancyNaturesSheet, BuildSheet(new[] { "OccupancyNatureDesc" }, new[] { "Residential Dwelling" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, ApplicantTypesSheet, BuildSheet(new[] { "ApplicantTypeDesc" }, new[] { "Owner" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, ProvincesSheet, BuildSheet(new[] { "ProvinceName" }, new[] { "Batangas" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, LgusSheet, BuildSheet(new[] { "ProvinceName", "LGUName" }, new[] { "Batangas", "Batangas City" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, BarangaysSheet, BuildSheet(new[] { "ProvinceName", "LGUName", "BarangayName" }, new[] { "Batangas", "Batangas City", "Pallocan West" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, RequirementCategoriesSheet, BuildSheet(new[] { "BuildingPermitReqCatDesc", "BuildingPermitCategoryName" }, new[] { "General Documents", "Simple" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, BuildingPermitRequirementsSheet, BuildSheet(new[] { "BuildingPermitReqCatDesc", "ReqDesc", "BuildingPermitCategoryName" }, new[] { "General Documents", "Signed application form", "Simple" }));
                AppendWorksheet(workbookPart, sheets, ref sheetId, CertificateOfOccupancyRequirementsSheet, BuildSheet(new[] { "ReqDesc" }, new[] { "Fire Safety Inspection Certificate (FSIC) from BFP" }));

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
            var items = await query
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
                    BuildingPermitCategoryIds = x.BuildingPermitCategoryId.HasValue ? new List<int> { x.BuildingPermitCategoryId.Value } : new List<int>(),
                    BuildingPermitCategoryNames = x.BuildingPermitCategory != null ? new List<string> { x.BuildingPermitCategory.CategoryName } : new List<string>(),
                    ParentId = x.ReqCatId,
                    ParentName = isCertificateOfOccupancy ? null : x.RequirementCategory.ReqCatDesc
                }).ToListAsync();

            if (isCertificateOfOccupancy)
            {
                return items;
            }

            return items
                .GroupBy(item => new
                {
                    item.Name,
                    item.ParentId,
                    item.ParentName
                })
                .Select(group =>
                {
                    var first = group.OrderBy(item => item.Id).First();
                    var categoryIds = group
                        .Where(item => item.BuildingPermitCategoryId.HasValue)
                        .Select(item => item.BuildingPermitCategoryId!.Value)
                        .Distinct()
                        .OrderBy(id => id)
                        .ToList();
                    var categoryNames = group
                        .Where(item => !string.IsNullOrWhiteSpace(item.BuildingPermitCategoryName))
                        .Select(item => item.BuildingPermitCategoryName!)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(name => name)
                        .ToList();

                    first.BuildingPermitCategoryIds = categoryIds;
                    first.BuildingPermitCategoryNames = categoryNames;
                    first.BuildingPermitCategoryId = categoryIds.Count == 1 ? categoryIds[0] : null;
                    first.BuildingPermitCategoryName = categoryNames.Count == 1 ? categoryNames[0] : null;
                    return first;
                })
                .OrderBy(item => item.ParentName)
                .ThenBy(item => item.Name)
                .ToList();
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

        private async Task InsertPermitApplicationTypeIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var name = RequiredValue(row, "PermitAppTypeDesc");
            var exists = await _context.PermitApplicationTypes.IgnoreQueryFilters().AnyAsync(x => x.PermitAppTypeDesc == name);
            if (exists)
            {
                RecordSkipped(summary, name);
                return;
            }

            _context.PermitApplicationTypes.Add(new PermitApplicationType { PermitAppTypeDesc = name, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, name);
        }

        private async Task InsertProjectClassificationIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var name = RequiredValue(row, "ProjectClassDesc");
            var exists = await _context.ProjectClassifications.IgnoreQueryFilters().AnyAsync(x => x.ProjectClassDesc == name);
            if (exists)
            {
                RecordSkipped(summary, name);
                return;
            }

            _context.ProjectClassifications.Add(new ProjectClassification { ProjectClassDesc = name, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, name);
        }

        private async Task InsertOwnershipTypeIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var name = RequiredValue(row, "OwnershipTypeDesc");
            var exists = await _context.OwnershipTypes.IgnoreQueryFilters().AnyAsync(x => x.OwnershipTypeDesc == name);
            if (exists)
            {
                RecordSkipped(summary, name);
                return;
            }

            _context.OwnershipTypes.Add(new OwnershipType { OwnershipTypeDesc = name, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, name);
        }

        private async Task InsertOccupancyNatureIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var name = RequiredValue(row, "OccupancyNatureDesc");
            var exists = await _context.OccupancyNatures.IgnoreQueryFilters().AnyAsync(x => x.OccupancyNatureDesc == name);
            if (exists)
            {
                RecordSkipped(summary, name);
                return;
            }

            _context.OccupancyNatures.Add(new OccupancyNature { OccupancyNatureDesc = name, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, name);
        }

        private async Task InsertApplicantTypeIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var name = RequiredValue(row, "ApplicantTypeDesc");
            var exists = await _context.ApplicantTypes.IgnoreQueryFilters().AnyAsync(x => x.ApplicantTypeDesc == name);
            if (exists)
            {
                RecordSkipped(summary, name);
                return;
            }

            _context.ApplicantTypes.Add(new ApplicantType { ApplicantTypeDesc = name, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, name);
        }

        private async Task InsertProvinceIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var name = RequiredValue(row, "ProvinceName");
            var exists = await _context.Provinces.IgnoreQueryFilters().AnyAsync(x => x.ProvinceName == name);
            if (exists)
            {
                RecordSkipped(summary, name);
                return;
            }

            _context.Provinces.Add(new Province { ProvinceName = name, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, name);
        }

        private async Task InsertLguIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var provinceName = RequiredValue(row, "ProvinceName");
            var lguName = RequiredValue(row, "LGUName");
            var province = await _context.Provinces.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceName == provinceName)
                ?? throw new InvalidOperationException($"Province '{provinceName}' does not exist.");

            var displayName = $"{lguName} ({provinceName})";
            var exists = await _context.LGUs.IgnoreQueryFilters().AnyAsync(x => x.ProvinceId == province.Id && x.LGUName == lguName);
            if (exists)
            {
                RecordSkipped(summary, displayName);
                return;
            }

            _context.LGUs.Add(new LGU { ProvinceId = province.Id, LGUName = lguName, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, displayName);
        }

        private async Task InsertBarangayIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var provinceName = RequiredValue(row, "ProvinceName");
            var lguName = RequiredValue(row, "LGUName");
            var barangayName = RequiredValue(row, "BarangayName");
            var province = await _context.Provinces.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceName == provinceName)
                ?? throw new InvalidOperationException($"Province '{provinceName}' does not exist.");
            var lgu = await _context.LGUs.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.ProvinceId == province.Id && x.LGUName == lguName)
                ?? throw new InvalidOperationException($"Municipality '{lguName}' does not exist in province '{provinceName}'.");

            var displayName = $"{barangayName} ({lguName}, {provinceName})";
            var exists = await _context.Barangays.IgnoreQueryFilters().AnyAsync(x => x.LGUId == lgu.Id && x.BarangayName == barangayName);
            if (exists)
            {
                RecordSkipped(summary, displayName);
                return;
            }

            _context.Barangays.Add(new Barangay { LGUId = lgu.Id, BarangayName = barangayName, CreatedAt = now, CreatedBy = actor });
            RecordAdded(summary, displayName);
        }

        private async Task InsertRequirementCategoryIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var categoryName = RequiredValue(row, "BuildingPermitReqCatDesc");
            row.TryGetValue("BuildingPermitCategoryName", out var buildingPermitCategoryName);
            var scope = MaintenanceApplicationScopes.BuildingPermit;
            var buildingPermitCategory = await ResolveBuildingPermitCategoryByNameAsync(buildingPermitCategoryName, scope);
            var buildingPermitCategoryId = buildingPermitCategory?.Id;
            var classification = await ResolveBuildingPermitClassificationAsync(buildingPermitCategoryId);

            var displayName = buildingPermitCategory != null
                ? $"{categoryName} ({buildingPermitCategory.CategoryName})"
                : categoryName;
            var exists = await _context.RequirementCategorys.IgnoreQueryFilters()
                .AnyAsync(x => x.ReqClassId == classification.Id && x.ReqCatDesc == categoryName && x.BuildingPermitCategoryId == buildingPermitCategoryId);
            if (exists)
            {
                RecordSkipped(summary, displayName);
                return;
            }

            _context.RequirementCategorys.Add(new RequirementCategory
            {
                ReqClassId = classification.Id,
                ReqCatDesc = categoryName,
                ApplicationTypeScope = scope,
                BuildingPermitCategoryId = buildingPermitCategoryId,
                CreatedAt = now,
                CreatedBy = actor
            });
            RecordAdded(summary, displayName);
        }

        private async Task InsertBuildingPermitRequirementIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var categoryName = RequiredValue(row, "BuildingPermitReqCatDesc");
            var reqName = RequiredValue(row, "ReqDesc");
            row.TryGetValue("BuildingPermitCategoryName", out var buildingPermitCategoryName);
            var scope = MaintenanceApplicationScopes.BuildingPermit;
            var buildingPermitCategory = await ResolveBuildingPermitCategoryByNameAsync(buildingPermitCategoryName, scope);
            var buildingPermitCategoryId = buildingPermitCategory?.Id;

            var category = await _context.RequirementCategorys.IgnoreQueryFilters()
                .Include(x => x.RequirementClassification)
                .FirstOrDefaultAsync(x =>
                    x.ReqCatDesc == categoryName
                    && x.BuildingPermitCategoryId == buildingPermitCategoryId
                    && x.ApplicationTypeScope == scope)
                ?? throw new InvalidOperationException(
                    buildingPermitCategory != null
                        ? $"Requirement category '{categoryName}' does not exist for building permit category '{buildingPermitCategory.CategoryName}'."
                        : $"Requirement category '{categoryName}' does not exist.");

            var displayName = buildingPermitCategory != null
                ? $"{reqName} \u2014 {categoryName} ({buildingPermitCategory.CategoryName})"
                : $"{reqName} \u2014 {categoryName}";
            var exists = await _context.Requirements.IgnoreQueryFilters()
                .AnyAsync(x => x.ReqCatId == category.Id && x.ReqDesc == reqName && x.BuildingPermitCategoryId == buildingPermitCategoryId);
            if (exists)
            {
                RecordSkipped(summary, displayName);
                return;
            }

            _context.Requirements.Add(new Requirement
            {
                ReqCatId = category.Id,
                ReqDesc = reqName,
                ApplicationTypeScope = scope,
                BuildingPermitCategoryId = buildingPermitCategoryId,
                CreatedAt = now,
                CreatedBy = actor
            });
            RecordAdded(summary, displayName);
        }

        private async Task InsertCertificateOfOccupancyRequirementIfMissingAsync(Dictionary<string, string> row, DateTime now, string actor, MaintenanceImportGroupSummaryDto summary)
        {
            var reqName = RequiredValue(row, "ReqDesc");
            var scope = MaintenanceApplicationScopes.CertificateOfOccupancy;

            var categoryId = await ResolveCertificateOfOccupancyGeneralCategoryIdAsync();
            var exists = await _context.Requirements.IgnoreQueryFilters()
                .AnyAsync(x => x.ReqCatId == categoryId && x.ReqDesc == reqName && x.BuildingPermitCategoryId == null);
            if (exists)
            {
                RecordSkipped(summary, reqName);
                return;
            }

            _context.Requirements.Add(new Requirement
            {
                ReqCatId = categoryId,
                ReqDesc = reqName,
                ApplicationTypeScope = scope,
                BuildingPermitCategoryId = null,
                CreatedAt = now,
                CreatedBy = actor
            });
            RecordAdded(summary, reqName);
        }

        private async Task<RequirementClassification> ResolveBuildingPermitClassificationAsync(int? buildingPermitCategoryId)
        {
            var bpClassifications = await _context.RequirementClassifications.IgnoreQueryFilters()
                .Where(x => x.ApplicationTypeScope == MaintenanceApplicationScopes.BuildingPermit)
                .ToListAsync();

            if (buildingPermitCategoryId.HasValue)
            {
                var scoped = bpClassifications.Where(x => x.BuildingPermitCategoryId == buildingPermitCategoryId).ToList();
                if (scoped.Count == 1)
                    return scoped[0];
                if (scoped.Count > 1)
                    throw new InvalidOperationException(
                        "Multiple Building Permit requirement classifications match this building permit category. Resolve the ambiguity in the management page before importing.");
            }

            var umbrella = bpClassifications.Where(x => x.BuildingPermitCategoryId == null).ToList();
            if (umbrella.Count == 1)
                return umbrella[0];
            if (umbrella.Count > 1)
                throw new InvalidOperationException(
                    "Multiple umbrella Building Permit requirement classifications exist. Resolve the ambiguity in the management page before importing.");

            throw new InvalidOperationException(
                "No Building Permit requirement classification is configured. Seed a classification before importing.");
        }

        private static void RecordAdded(MaintenanceImportGroupSummaryDto summary, string name)
        {
            summary.CreatedCount++;
            summary.AddedNames.Add(name);
        }

        private static void RecordSkipped(MaintenanceImportGroupSummaryDto summary, string name)
        {
            summary.SkippedCount++;
            summary.SkippedNames.Add(name);
        }

        private static string RequiredValue(Dictionary<string, string> row, string key)
        {
            if (!row.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"'{key}' is required.");

            return value.Trim();
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
