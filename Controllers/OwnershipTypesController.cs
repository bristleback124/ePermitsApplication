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
    [Route("api/ownershiptypes")]
    public class OwnershipTypesController : ControllerBase
    {
        private readonly IOwnershipTypeService _service;
        private readonly IMapper _mapper;

        public OwnershipTypesController(
            IOwnershipTypeService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OwnershipTypeDto>>> GetAll([FromQuery] bool activeOnly = false)
        {
            var ownershipTypes = await _service.GetAllAsync();
            if (activeOnly)
                ownershipTypes = ownershipTypes.Where(x => x.IsActive);
            return Ok(_mapper.Map<IEnumerable<OwnershipTypeDto>>(ownershipTypes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OwnershipTypeDto>> GetById(int id)
        {
            var ownershipType = await _service.GetByIdAsync(id);
            if (ownershipType == null)
                return NotFound();

            return Ok(_mapper.Map<OwnershipTypeDto>(ownershipType));
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateOwnershipTypeDto dto)
        {
            var ownershipType = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = ownershipType.Id },
                _mapper.Map<OwnershipTypeDto>(ownershipType));
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateOwnershipTypeDto dto)
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
        public async Task<ActionResult<IEnumerable<OwnershipTypeDto>>> GetByName(
            [FromQuery] string ownershipTypeDesc,
            [FromQuery] PaginationParams pagination)
        {
            var ownershipTypes = await _service.GetByNameAsync(ownershipTypeDesc, pagination);
            return Ok(_mapper.Map<IEnumerable<OwnershipTypeDto>>(ownershipTypes));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<OwnershipTypeDto>>> Filter(
            [FromQuery] string ownershipTypeDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByNameAsync(ownershipTypeDesc, pagination);

            return Ok(new PagedResult<OwnershipTypeDto>
            {
                Items = _mapper.Map<IEnumerable<OwnershipTypeDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
