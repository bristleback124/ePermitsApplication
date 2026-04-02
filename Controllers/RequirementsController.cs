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
    [Route("api/requirements")]
    public class RequirementsController : ControllerBase
    {
        private readonly IRequirementService _service;
        private readonly IMapper _mapper;

        public RequirementsController(IRequirementService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementDto>>> GetAll()
        {
            var reqs = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<RequirementDto>>(reqs));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementDto>> GetById(int id)
        {
            var req = await _service.GetByIdAsync(id);
            if (req == null)
                return NotFound();

            return Ok(_mapper.Map<RequirementDto>(req));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateRequirementDto dto)
        {
            var req = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = req.Id },
                _mapper.Map<RequirementDto>(req));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateRequirementDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _service.RestoreAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<RequirementDto>>> Filter(
            [FromQuery] string? reqDesc,
            [FromQuery] int? reqCatId,
            [FromQuery] string? reqClassDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterAsync(
                reqDesc, reqCatId, reqClassDesc, pagination);

            return Ok(new PagedResult<RequirementDto>
            {
                Items = _mapper.Map<IEnumerable<RequirementDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
