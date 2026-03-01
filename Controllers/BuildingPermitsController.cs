using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/building-permits")]
    public class BuildingPermitsController : ControllerBase
    {
        private readonly IBuildingPermitService _service;
        private readonly IMapper _mapper;

        public BuildingPermitsController(
            IBuildingPermitService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<BuildingPermitDto>>> GetAll([FromQuery] PaginationParams pagination)
        {
            var result = await _service.GetAllAsync(pagination);

            return Ok(new PagedResult<BuildingPermitDto>
            {
                Items = _mapper.Map<IEnumerable<BuildingPermitDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuildingPermitDto>> GetById(int id)
        {
            var buildingPermit = await _service.GetByIdAsync(id);
            if (buildingPermit == null)
                return NotFound();

            return Ok(_mapper.Map<BuildingPermitDto>(buildingPermit));
        }

        [HttpPost]
        public async Task<ActionResult<BuildingPermitDto>> Create([FromForm] BuildingPermitCreateDto dto)
        {
            var buildingPermit = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = buildingPermit.Id },
                _mapper.Map<BuildingPermitDto>(buildingPermit));
        }
    }
}
