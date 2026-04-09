using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities.CoOApp;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/coo-apps")]
    [Authorize]
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
        public async Task<ActionResult<CoOAppDto>> Create([FromForm] CoOAppCreateDto dto, [FromQuery] bool saveAsDraft = false)
        {
            try
            {
                var coOApp = await _service.CreateAsync(dto, saveAsDraft);

                return CreatedAtAction(nameof(GetById),
                    new { id = coOApp.Id },
                    _mapper.Map<CoOAppDto>(coOApp));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("application/{applicationId}/edit")]
        public async Task<ActionResult<CoOAppEditDto>> GetEditByApplicationId(int applicationId)
        {
            var coOApp = await _service.GetEditByApplicationIdAsync(applicationId);
            if (coOApp == null)
                return NotFound();

            return Ok(coOApp);
        }

        [HttpGet("application/{applicationId}/form")]
        public async Task<ActionResult<CoOAppEditDto>> GetFormByApplicationId(int applicationId)
        {
            var coOApp = await _service.GetFormByApplicationIdAsync(applicationId);
            if (coOApp == null)
                return NotFound();

            return Ok(coOApp);
        }

        [HttpPut("application/{applicationId}")]
        public async Task<IActionResult> UpdateByApplicationId(int applicationId, [FromForm] CoOAppUpdateDto dto, [FromQuery] bool saveAsDraft = false)
        {
            try
            {
                var result = await _service.UpdateByApplicationIdAsync(applicationId, dto, saveAsDraft);
                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = result.Message, data = _mapper.Map<CoOAppDto>(result.CoOApp) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
