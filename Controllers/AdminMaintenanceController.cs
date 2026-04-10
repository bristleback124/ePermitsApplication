using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/admin/maintenance")]
    [Authorize(Roles = "admin,superadmin,sysadmin")]
    public class AdminMaintenanceController : ControllerBase
    {
        private readonly IAdminMaintenanceService _service;

        public AdminMaintenanceController(IAdminMaintenanceService service)
        {
            _service = service;
        }

        [HttpGet("{entityType}")]
        public async Task<ActionResult<IEnumerable<MaintenanceLookupItemDto>>> GetItems(
            string entityType,
            [FromQuery] bool includeInactive = true,
            [FromQuery] bool includeDeleted = false,
            [FromQuery] string? search = null,
            [FromQuery] string? applicationType = null)
        {
            var items = await _service.GetLookupItemsAsync(
                entityType,
                includeInactive,
                includeDeleted,
                search,
                applicationType);

            return Ok(items);
        }

        [HttpPost("{entityType}")]
        public async Task<ActionResult<object>> CreateItem(
            string entityType,
            [FromBody] CreateMaintenanceItemDto dto)
        {
            try
            {
                var item = await _service.CreateItemAsync(entityType, new Dictionary<string, object?>
                {
                    ["name"] = dto.Name,
                    ["parentId"] = dto.ParentId,
                    ["applicationTypeScope"] = dto.ApplicationTypeScope,
                    ["buildingPermitCategoryId"] = dto.BuildingPermitCategoryId
                });

                return Ok(item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("{entityType}/{id}/status")]
        public async Task<ActionResult<MaintenanceLookupItemDto>> UpdateStatus(
            string entityType,
            int id,
            [FromBody] UpdateMaintenanceStatusDto dto)
        {
            try
            {
                var item = await _service.SetActiveStatusAsync(entityType, id, dto.IsActive);
                return Ok(item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{entityType}/{id}/usage")]
        public async Task<ActionResult<MaintenanceReferenceUsageDto>> GetUsage(string entityType, int id)
        {
            try
            {
                return Ok(await _service.GetReferenceUsageAsync(entityType, id));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var stream = await _service.GenerateTemplateAsync();
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "maintenance-import-template.xlsx");
        }

        [HttpPost("import")]
        [RequestSizeLimit(25_000_000)]
        public async Task<ActionResult<MaintenanceImportResultDto>> Import([FromForm] MaintenanceImportRequestDto request)
        {
            try
            {
                var result = await _service.ImportAsync(request);
                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
