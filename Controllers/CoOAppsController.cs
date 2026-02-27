using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.CoOApp;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/coo-apps")]
    public class CoOAppsController : ControllerBase
    {
        private readonly ICoOAppService _service;
        private readonly IMapper _mapper;

        public CoOAppsController(
            ICoOAppService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<CoOAppDto>>> GetAll([FromQuery] PaginationParams pagination)
        {
            var result = await _service.GetAllAsync(pagination);

            return Ok(new PagedResult<CoOAppDto>
            {
                Items = _mapper.Map<IEnumerable<CoOAppDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoOAppDto>> GetById(int id)
        {
            var coOApp = await _service.GetByIdAsync(id);
            if (coOApp == null)
                return NotFound();

            return Ok(_mapper.Map<CoOAppDto>(coOApp));
        }

        [HttpPost]
        public async Task<ActionResult<CoOAppDto>> Create([FromForm] CoOAppCreateDto dto)
        {
            var coOApp = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = coOApp.Id },
                _mapper.Map<CoOAppDto>(coOApp));
        }
    }
}
