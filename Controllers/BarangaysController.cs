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
    [Route("api/barangays")]    
    public class BarangaysController : ControllerBase
    {
        private readonly IBarangayService _service;
        private readonly IMapper _mapper;

        public BarangaysController(IBarangayService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BarangayDto>>> GetAll()
        {
            var barangays = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<BarangayDto>>(barangays));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BarangayDto>> GetById(int id)
        {
            var barangay = await _service.GetByIdAsync(id);
            if (barangay == null)
                return NotFound();
                        
            return Ok(_mapper.Map<BarangayDto>(barangay));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateBarangayDto dto)
        {
            var barangay = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = barangay.Id },
                _mapper.Map<BarangayDto>(barangay));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateBarangayDto dto)
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
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _service.RestoreAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }            

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<BarangayDto>>> Filter(
            [FromQuery] string? barangayName,
            [FromQuery] int? lguId,
            [FromQuery] string? provinceName,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterAsync(
                barangayName, lguId, provinceName, pagination);

            return Ok(new PagedResult<BarangayDto>
            {
                Items = _mapper.Map<IEnumerable<BarangayDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
