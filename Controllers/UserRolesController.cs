using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ePermitsApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user-roles")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _service;
        private readonly IMapper _mapper;

        public UserRolesController(IUserRoleService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetAll()
        {
            var userRoles = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<UserRoleDto>>(userRoles));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoleDto>> GetById(int id)
        {
            var userRole = await _service.GetByIdAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserRoleDto>(userRole));
        }

        [HttpGet("by-description")]
        public async Task<ActionResult<UserRoleDto>> GetByDescription([FromQuery] string description)
        {
            var userRole = await _service.GetByDescriptionAsync(description);
            if (userRole == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserRoleDto>(userRole));
        }

        [HttpPost]
        public async Task<ActionResult<UserRoleDto>> Create(CreateUserRoleDto dto)
        {
            var userRole = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = userRole.Id },
                _mapper.Map<UserRoleDto>(userRole));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserRoleDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
