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
    [Route("api/[controller]")]
    public class ProvincesController : ControllerBase
    {
        private readonly IProvinceService _service;
        private readonly IMapper _mapper;

        public ProvincesController(
            IProvinceService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProvinceDto>>> GetAll([FromQuery] bool activeOnly = false)
        {
            var provinces = await _service.GetAllAsync();
            if (activeOnly)
                provinces = provinces.Where(x => x.IsActive);
            return Ok(_mapper.Map<IEnumerable<ProvinceDto>>(provinces));
        }

        //[HttpGet]
        //public async Task<ActionResult<PagedResult<ProvinceDto>>> GetAll(
        //[FromQuery] string? name,
        //[FromQuery] PaginationParams pagination)
        //{
        //    var result = await _service.FilterByNameAsync(name ?? "", pagination);
        //    return Ok(new PagedResult<ProvinceDto>
        //    {
        //        Items = _mapper.Map<IEnumerable<ProvinceDto>>(result.Items),
        //        TotalCount = result.TotalCount,
        //        PageNumber = result.PageNumber,
        //        PageSize = result.PageSize
        //    });
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<ProvinceDto>> GetById(int id)
        {
            var province = await _service.GetByIdAsync(id);
            if (province == null)
                return NotFound();

            return Ok(_mapper.Map<ProvinceDto>(province));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateProvinceDto dto)
        {
            var province = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = province.Id },
                _mapper.Map<ProvinceDto>(province));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateProvinceDto dto)
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
        public async Task<ActionResult<IEnumerable<ProvinceDto>>> GetByName(
            [FromQuery] string provinceName,
            [FromQuery] PaginationParams pagination)
        {
            var provinces = await _service.GetByNameAsync(provinceName, pagination);
            return Ok(_mapper.Map<IEnumerable<ProvinceDto>>(provinces));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<ProvinceDto>>> Filter(
            [FromQuery] string provinceName,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByNameAsync(provinceName, pagination);

            return Ok(new PagedResult<ProvinceDto>
            {
                Items = _mapper.Map<IEnumerable<ProvinceDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
