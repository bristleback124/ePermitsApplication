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
    [Route("api/applicant-types")]    
    public class ApplicantTypesController : ControllerBase
    {
        private readonly IApplicantTypeService _service;
        private readonly IMapper _mapper;

        public ApplicantTypesController(
            IApplicantTypeService service,
            IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicantTypeDto>>> GetAll()
        {
            var applicantTypes = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ApplicantTypeDto>>(applicantTypes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicantTypeDto>> GetById(int id)
        {
            var applicantType = await _service.GetByIdAsync(id);
            if (applicantType == null)
                return NotFound();

            return Ok(_mapper.Map<ApplicantTypeDto>(applicantType));
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateApplicantTypeDto dto)
        {
            var applicantType = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = applicantType.Id },
                _mapper.Map<ApplicantTypeDto>(applicantType));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateApplicantTypeDto dto)
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

        [HttpGet("getbyname")]
        public async Task<ActionResult<IEnumerable<ApplicantTypeDto>>> GetByName(
            [FromQuery] string applicantTypeDesc,
            [FromQuery] PaginationParams pagination)
        {
            var applicantTypes = await _service.GetByNameAsync(applicantTypeDesc, pagination);
            return Ok(_mapper.Map<IEnumerable<ApplicantTypeDto>>(applicantTypes));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<ApplicantTypeDto>>> Filter(
            [FromQuery] string applicantTypeDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByNameAsync(applicantTypeDesc, pagination);

            return Ok(new PagedResult<ApplicantTypeDto>
            {
                Items = _mapper.Map<IEnumerable<ApplicantTypeDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
