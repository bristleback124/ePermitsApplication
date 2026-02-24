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
    [Route("api/departments")]    
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly IMapper _mapper;

        public DepartmentsController(IDepartmentService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
        {
            var departments = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departments));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var department = await _service.GetByIdAsync(id);
            if (department == null) 
                return NotFound();

            return Ok(_mapper.Map<DepartmentDto>(department));
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> Create(CreateDepartmentDto dto)
        {
            var department = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = department.Id },
                _mapper.Map<DepartmentDto>(department));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var success = await _service.SoftDeleteAsync(id);
            //return success ? NoContent() : NotFound();
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
        public async Task<ActionResult<PagedResult<DepartmentDto>>> Filter(
            [FromQuery] string? departmentName,
            [FromQuery] string? departmentCode,
            [FromQuery] int? lguId,
            [FromQuery] string? provinceName,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterAsync(
                departmentName, departmentCode, lguId, provinceName, pagination);

            return Ok(new PagedResult<DepartmentDto>
            {
                Items = _mapper.Map<IEnumerable<DepartmentDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
