using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.BuildingPermit;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/building-permits")]
    [Authorize]
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
        [RequestSizeLimit(200 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 200 * 1024 * 1024)]
        public async Task<ActionResult<BuildingPermitDto>> Create([FromForm] BuildingPermitCreateDto dto, [FromQuery] bool saveAsDraft = false, [FromQuery] int? applicantId = null)
        {
            try
            {
                var buildingPermit = await _service.CreateAsync(dto, saveAsDraft, applicantId);

                return CreatedAtAction(nameof(GetById),
                    new { id = buildingPermit.Id },
                    _mapper.Map<BuildingPermitDto>(buildingPermit));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("application/{applicationId}/edit")]
        public async Task<ActionResult<BuildingPermitEditDto>> GetEditByApplicationId(int applicationId)
        {
            var buildingPermit = await _service.GetEditByApplicationIdAsync(applicationId);
            if (buildingPermit == null)
                return NotFound();

            return Ok(buildingPermit);
        }

        [HttpGet("application/{applicationId}/form")]
        public async Task<ActionResult<BuildingPermitEditDto>> GetFormByApplicationId(int applicationId)
        {
            var buildingPermit = await _service.GetFormByApplicationIdAsync(applicationId);
            if (buildingPermit == null)
                return NotFound();

            return Ok(buildingPermit);
        }

        [HttpPut("application/{applicationId}")]
        [RequestSizeLimit(200 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 200 * 1024 * 1024)]
        public async Task<IActionResult> UpdateByApplicationId(int applicationId, [FromForm] BuildingPermitUpdateDto dto, [FromQuery] bool saveAsDraft = false)
        {
            try
            {
                var result = await _service.UpdateByApplicationIdAsync(applicationId, dto, saveAsDraft);
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = result.Message, data = _mapper.Map<BuildingPermitDto>(result.BuildingPermit) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
