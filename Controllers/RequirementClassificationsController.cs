using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/requirementclassifications")]
    public class RequirementClassificationsController : ControllerBase
    {
        private readonly IRequirementClassificationService _service;
        private readonly IMapper _mapper;

        public RequirementClassificationsController(
            IRequirementClassificationService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("hierarchy")]
        public async Task<ActionResult<IEnumerable<RequirementClassificationHierarchyDto>>> GetHierarchy(
            [FromQuery] string? applicationType = null,
            [FromQuery] int? buildingPermitCategoryId = null,
            [FromQuery] bool activeOnly = true)
        {
            var classifications = await _service.GetAllWithHierarchyAsync();
            if (activeOnly)
            {
                classifications = classifications
                    .Where(c => c.IsActive)
                    .Select(c =>
                    {
                        c.RequirementCategorys = c.RequirementCategorys
                            .Where(cat => cat.IsActive)
                            .Select(cat =>
                            {
                                cat.Requirements = cat.Requirements
                                    .Where(req => req.IsActive)
                                    .ToList();
                                return cat;
                            })
                            .ToList();
                        return c;
                    })
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(applicationType))
            {
                classifications = classifications
                    .Where(c => MaintenanceApplicationScopes.Matches(c.ApplicationTypeScope, applicationType))
                    .Where(c =>
                        string.Equals(applicationType, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase) == false
                        || buildingPermitCategoryId == null
                        || c.BuildingPermitCategoryId == null
                        || c.BuildingPermitCategoryId == buildingPermitCategoryId)
                    .Select(c =>
                    {
                        c.RequirementCategorys = c.RequirementCategorys
                            .Where(cat => MaintenanceApplicationScopes.Matches(cat.ApplicationTypeScope, applicationType))
                            .Where(cat =>
                                string.Equals(applicationType, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase) == false
                                || buildingPermitCategoryId == null
                                || cat.BuildingPermitCategoryId == null
                                || cat.BuildingPermitCategoryId == buildingPermitCategoryId)
                            .Select(cat =>
                            {
                                cat.Requirements = cat.Requirements
                                    .Where(req => MaintenanceApplicationScopes.Matches(req.ApplicationTypeScope, applicationType))
                                    .Where(req =>
                                        string.Equals(applicationType, MaintenanceApplicationScopes.BuildingPermit, StringComparison.OrdinalIgnoreCase) == false
                                        || buildingPermitCategoryId == null
                                        || req.BuildingPermitCategoryId == null
                                        || req.BuildingPermitCategoryId == buildingPermitCategoryId)
                                    .ToList();
                                return cat;
                            })
                            .Where(cat => cat.Requirements.Count > 0)
                            .ToList();
                        return c;
                    })
                    .Where(c => c.RequirementCategorys.Count > 0)
                    .ToList();
            }
            return Ok(_mapper.Map<IEnumerable<RequirementClassificationHierarchyDto>>(classifications));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementClassificationDto>>> GetAll()
        {
            var reqClassifications = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<RequirementClassificationDto>>(reqClassifications));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementClassificationDto>> GetById(int id)
        {
            var reqClassification = await _service.GetByIdAsync(id);
            if (reqClassification == null)
                return NotFound();

            return Ok(_mapper.Map<RequirementClassificationDto>(reqClassification));
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateRequirementClassificationDto dto)
        {
            var reqClassification = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = reqClassification.Id },
                _mapper.Map<RequirementClassificationDto>(reqClassification));
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateRequirementClassificationDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            try
            {
                var success = await _service.SoftDeleteAsync(id);
                if (!success)
                    return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }

            return NoContent();
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpPost("{id}/restore")]
        public async Task<ActionResult> Restore(int id)
        {
            var success = await _service.RestoreAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("getbyname")]
        public async Task<ActionResult<IEnumerable<RequirementClassificationDto>>> GetByName(
            [FromQuery] string reqClassDesc,
            [FromQuery] PaginationParams pagination)
        {
            var reqClassifications = await _service.GetByNameAsync(reqClassDesc, pagination);
            return Ok(_mapper.Map<IEnumerable<RequirementClassificationDto>>(reqClassifications));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<RequirementClassificationDto>>> Filter(
            [FromQuery] string reqClassDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByNameAsync(reqClassDesc, pagination);

            return Ok(new PagedResult<RequirementClassificationDto>
            {
                Items = _mapper.Map<IEnumerable<RequirementClassificationDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
