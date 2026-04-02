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
    [Route("api/permitapplicationtypes")]
    public class PermitApplicationTypesController : ControllerBase
    {
        private readonly IPermitApplicationTypeService _service;
        private readonly IMapper _mapper;

        public PermitApplicationTypesController(
            IPermitApplicationTypeService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermitApplicationTypeDto>>> GetAll([FromQuery] bool activeOnly = false)
        {
            var permitApplicationTypes = await _service.GetAllAsync();
            if (activeOnly)
                permitApplicationTypes = permitApplicationTypes.Where(x => x.IsActive);
            return Ok(_mapper.Map<IEnumerable<PermitApplicationTypeDto>>(permitApplicationTypes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermitApplicationTypeDto>> GetById(int id)
        {
            var permitApplicationType = await _service.GetByIdAsync(id);
            if (permitApplicationType == null)
                return NotFound();

            return Ok(_mapper.Map<PermitApplicationTypeDto>(permitApplicationType));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Create(CreatePermitApplicationTypeDto dto)
        {
            var permitApplicationType = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = permitApplicationType.Id },
                _mapper.Map<PermitApplicationTypeDto>(permitApplicationType));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdatePermitApplicationTypeDto dto)
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
        public async Task<ActionResult<IEnumerable<PermitApplicationTypeDto>>> GetByName(
            [FromQuery] string permitAppTypeDesc,
            [FromQuery] PaginationParams pagination)
        {
            var permitApplicationTypes = await _service.GetByNameAsync(permitAppTypeDesc, pagination);
            return Ok(_mapper.Map<IEnumerable<PermitApplicationTypeDto>>(permitApplicationTypes));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<PermitApplicationTypeDto>>> Filter(
            [FromQuery] string permitAppTypeDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByNameAsync(permitAppTypeDesc, pagination);

            return Ok(new PagedResult<PermitApplicationTypeDto>
            {
                Items = _mapper.Map<IEnumerable<PermitApplicationTypeDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
