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
    [Route("api/projectclassifications")]
    public class ProjectClassificationsController : ControllerBase
    {
        private readonly IProjectClassificationService _service;
        private readonly IMapper _mapper;

        public ProjectClassificationsController(
            IProjectClassificationService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectClassificationDto>>> GetAll()
        {
            var projectClassifications = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ProjectClassificationDto>>(projectClassifications));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectClassificationDto>> GetById(int id)
        {
            var projectClassification = await _service.GetByIdAsync(id);
            if (projectClassification == null)
                return NotFound();

            return Ok(_mapper.Map<ProjectClassificationDto>(projectClassification));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateProjectClassificationDto dto)
        {
            var projectClassification = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = projectClassification.Id },
                _mapper.Map<ProjectClassificationDto>(projectClassification));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateProjectClassificationDto dto)
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
        public async Task<ActionResult<IEnumerable<ProjectClassificationDto>>> GetByName(
            [FromQuery] string projectClassDesc,
            [FromQuery] PaginationParams pagination)
        {
            var projectClassifications = await _service.GetByNameAsync(projectClassDesc, pagination);
            return Ok(_mapper.Map<IEnumerable<ProjectClassificationDto>>(projectClassifications));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<ProjectClassificationDto>>> Filter(
            [FromQuery] string projectClassDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByNameAsync(projectClassDesc, pagination);

            return Ok(new PagedResult<ProjectClassificationDto>
            {
                Items = _mapper.Map<IEnumerable<ProjectClassificationDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
