using ePermitsApp.Data;
using ePermitsApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Controllers
{
    [ApiController]
    [Route("api/building-permit-categories")]
    public class BuildingPermitCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BuildingPermitCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildingPermitCategoryDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            var query = includeInactive
                ? _context.BuildingPermitCategories.IgnoreQueryFilters()
                : _context.BuildingPermitCategories.AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            var items = await query
                .OrderBy(x => x.Id)
                .Select(x => new BuildingPermitCategoryDto
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return Ok(items);
        }
    }
}
