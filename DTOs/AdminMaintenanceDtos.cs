using Microsoft.AspNetCore.Http;

namespace ePermitsApp.DTOs
{
    public class MaintenanceLookupItemDto
    {
        public string EntityType { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? ApplicationTypeScope { get; set; }
        public int? BuildingPermitCategoryId { get; set; }
        public string? BuildingPermitCategoryName { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
    }

    public class UpdateMaintenanceStatusDto
    {
        public bool IsActive { get; set; }
    }

    public class MaintenanceReferenceUsageDto
    {
        public string EntityType { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalReferences { get; set; }
        public int ActiveApplicationReferences { get; set; }
        public bool CanSoftDelete => TotalReferences == 0;
        public List<MaintenanceReferenceUsageItemDto> Usages { get; set; } = new();
    }

    public class MaintenanceReferenceUsageItemDto
    {
        public string Source { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MaintenanceImportRequestDto
    {
        public IFormFile File { get; set; } = null!;
    }

    public class CreateMaintenanceItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ApplicationTypeScope { get; set; }
        public int? BuildingPermitCategoryId { get; set; }
    }

    public class MaintenanceImportResultDto
    {
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int ReactivatedCount { get; set; }
        public bool Succeeded => Errors.Count == 0;
        public List<MaintenanceImportErrorDto> Errors { get; set; } = new();
    }

    public class MaintenanceImportErrorDto
    {
        public string SheetName { get; set; } = string.Empty;
        public int RowNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
