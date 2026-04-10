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
    [Route("api/requirementcategory")]

    public class RequirementCategoryController : ControllerBase
    {
        private readonly IRequirementCategoryService _service;
        private readonly IMapper _mapper;

        public RequirementCategoryController(IRequirementCategoryService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementCategoryDto>>> GetAll()
        {
            var reqCats = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<RequirementCategoryDto>>(reqCats));
        }               

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementCategoryDto>> GetById(int id)
        {
            var reqCat = await _service.GetByIdAsync(id);
            if (reqCat == null)
                return NotFound();

            return Ok(_mapper.Map<RequirementCategoryDto>(reqCat));
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpPost]
        public async Task<ActionResult> Create(CreateRequirementCategoryDto dto)
        {
            var reqCat = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById),
                new { id = reqCat.Id },
                _mapper.Map<RequirementCategoryDto>(reqCat));
        }

        [Authorize(Roles = "admin,superadmin,sysadmin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateRequirementCategoryDto dto)
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

        [HttpGet("byreqclass/{reqClassId}")]
        public async Task<ActionResult<IEnumerable<RequirementCategoryDto>>> GetByReqClass(int reqClassId)
        {
            var reqCats = await _service.GetByReqClassAsync(reqClassId);
            return Ok(_mapper.Map<IEnumerable<RequirementCategoryDto>>(reqCats));
        }

        [HttpGet("byreqclassdesc")]
        public async Task<ActionResult<IEnumerable<RequirementCategoryDto>>> GetByReqClassDesc(
            [FromQuery] string reqClassDesc,
            [FromQuery] PaginationParams pagination)
        {
            var reqCats = await _service.GetByReqClassDescAsync(reqClassDesc, pagination);
            return Ok(_mapper.Map<IEnumerable<RequirementCategoryDto>>(reqCats));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<RequirementCategoryDto>>> Filter(
            [FromQuery] string reqClassDesc,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _service.FilterByReqClassDescAsync(reqClassDesc, pagination);

            return Ok(new PagedResult<RequirementCategoryDto>
            {
                Items = _mapper.Map<IEnumerable<RequirementCategoryDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            });
        }
    }
}
