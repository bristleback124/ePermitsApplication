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
    [Route("api/occupancynatures")]
    public class OccupancyNaturesController : ControllerBase
    {
        private readonly IOccupancyNatureService _service;
        private readonly IMapper _mapper;

        public OccupancyNaturesController(
            IOccupancyNatureService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OccupancyNatureDto>>> GetAll([FromQuery] bool activeOnly = false)
        {
            var occupancyNatures = await _service.GetAllAsync();
            if (activeOnly)
                occupancyNatures = occupancyNatures.Where(x => x.IsActive);
            return Ok(_mapper.Map<IEnumerable<OccupancyNatureDto>>(occupancyNatures));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OccupancyNatureDto>> GetById(int id)
        {
            var occupancyNature = await _service.GetByIdAsync(id);
            if (occupancyNature == null)
                return NotFound();

            return Ok(_mapper.Map<OccupancyNatureDto>(occupancyNature));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateOccupancyNatureDto dto)
        {
            var occupancyNature = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = occupancyNature.Id },
                _mapper.Map<OccupancyNatureDto>(occupancyNature));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateOccupancyNatureDto dto)
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
        public async Task<ActionResult> Restore(int id)
        {
            var success = await _service.RestoreAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpGet("getbyname")]
        public async Task<ActionResult<IEnumerable<OccupancyNatureDto>>> GetByName(
            [FromQuery] string occupancyNatureDesc,
            [FromQuery] PaginationParams pagination)
        {
            var occupancyNatures = await _service.GetByNameAsync(occupancyNatureDesc, pagination);
            return Ok(_mapper.Map<IEnumerable<OccupancyNatureDto>>(occupancyNatures));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<OccupancyNatureDto>>> Filter(
            [FromQuery] string occupancyNatureDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByNameAsync(occupancyNatureDesc, pagination);

            return Ok(new PagedResult<OccupancyNatureDto>
            {
                Items = _mapper.Map<IEnumerable<OccupancyNatureDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
