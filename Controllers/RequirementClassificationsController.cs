using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
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
        public async Task<ActionResult<IEnumerable<RequirementClassificationHierarchyDto>>> GetHierarchy()
        {
            var classifications = await _service.GetAllWithHierarchyAsync();
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

        [HttpPost]
        public async Task<ActionResult> Create(CreateRequirementClassificationDto dto)
        {
            var reqClassification = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = reqClassification.Id },
                _mapper.Map<RequirementClassificationDto>(reqClassification));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateRequirementClassificationDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var success = await _service.SoftDeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

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
