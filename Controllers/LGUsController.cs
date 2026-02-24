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
    public class LGUsController : ControllerBase
    {
        private readonly ILGUService _service;
        private readonly IMapper _mapper;

        public LGUsController(ILGUService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LGUDto>>> GetAll()
        {
            var lgus = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<LGUDto>>(lgus));
        }

       // [HttpGet]
       // public async Task<ActionResult<PagedResult<LGUDto>>> GetAll(
       //[FromQuery] int? provinceId,
       //[FromQuery] string? provinceName,
       //[FromQuery] PaginationParams pagination)
       // {
       //     PagedResult<LGU> result;
       //     if (provinceId.HasValue)
       //         result = await _service.GetByProvinceAsync(provinceId.Value, pagination);
       //     else
       //         result = await _service.GetByProvinceNameAsync(provinceName ?? "", pagination);

       //     return Ok(new PagedResult<LGUDto>
       //     {
       //         Items = _mapper.Map<IEnumerable<LGUDto>>(result.Items),
       //         TotalCount = result.TotalCount,
       //         PageNumber = result.PageNumber,
       //         PageSize = result.PageSize
       //     });
       // }

        [HttpGet("{id}")]
        public async Task<ActionResult<LGUDto>> GetById(int id)
        {
            var lgu = await _service.GetByIdAsync(id);
            if (lgu == null)
                return NotFound();

            return Ok(_mapper.Map<LGUDto>(lgu));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateLGUDto dto)
        {
            var lgu = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = lgu.Id },
                _mapper.Map<LGUDto>(lgu));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateLGUDto dto)
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

        [HttpGet("by-province/{provinceId}")]
        public async Task<ActionResult<IEnumerable<LGUDto>>> GetByProvince(int provinceId)
        {
            var lgus = await _service.GetByProvinceAsync(provinceId);
            return Ok(_mapper.Map<IEnumerable<LGUDto>>(lgus));
        }

        [HttpGet("by-provincename")]
        public async Task<ActionResult<IEnumerable<LGUDto>>> GetByProvinceName(
            [FromQuery] string provinceName,
            [FromQuery] PaginationParams pagination)
        {
            var lgus = await _service.GetByProvinceNameAsync(provinceName, pagination);
            return Ok(_mapper.Map<IEnumerable<LGUDto>>(lgus));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<LGUDto>>> Filter(
            [FromQuery] string provinceName,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByProvinceNameAsync(provinceName, pagination);

            return Ok(new PagedResult<LGUDto>
            {
                Items = _mapper.Map<IEnumerable<LGUDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
