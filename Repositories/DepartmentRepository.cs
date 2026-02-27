using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.LGU)
                    .ThenInclude(l => l.Province)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.LGU)
                    .ThenInclude(l => l.Province)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
        } 

        public void Update(Department department)
        {
            _context.Departments.Update(department);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        
        public async Task<Department?> GetByIdIncludingDeletedAsync(int id)
        {
             return await _context.Departments
                .IgnoreQueryFilters()
                .Include(d => d.LGU)
                    .ThenInclude(l => l.Province)
                .FirstOrDefaultAsync(d => d.Id == id);
        } 

        public async Task<PagedResult<Department>> FilterAsync(
            string? departmentName,
            string? departmentCode,
            int? lguId,
            string? provinceName,
            PaginationParams pagination)
        {
            var query = _context.Departments
                .Include(d => d.LGU)
                    .ThenInclude(l => l.Province)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(departmentName))
                query = query.Where(d => 
                    d.DepartmentName.ToLower().Contains(departmentName.ToLower()));

            if (!string.IsNullOrWhiteSpace(departmentCode))
                query = query.Where(d => 
                    d.DepartmentCode.ToLower().Contains(departmentCode.ToLower()));

            if (lguId.HasValue)
                query = query.Where(d => d.LGUId == lguId);

            if (!string.IsNullOrWhiteSpace(provinceName))
                query = query.Where(d => 
                    d.LGU.Province.ProvinceName.ToLower()
                        .Contains(provinceName.ToLower()));

            query = pagination.OrderBy?.ToLower() switch
            {
                "departmentname" => pagination.IsDescending 
                    ? query.OrderByDescending(d => d.DepartmentName) 
                    : query.OrderBy(d => d.DepartmentName),
                "departmentcode" => pagination.IsDescending 
                    ? query.OrderByDescending(d => d.DepartmentCode) 
                    : query.OrderBy(d => d.DepartmentCode),

                _ => query.OrderBy(d => d.Id)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Department>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }

        public async Task<IEnumerable<Department>> GetDeletedByLGUAsync(int lguId)
        {
            return await _context.Departments
                .IgnoreQueryFilters()
                .Where(d => d.LGUId == lguId && d.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Department>> GetByLGUAsync(int lguId)
        {
            return await _context.Departments
                .Where(d => d.LGUId == lguId)
                .ToListAsync();
        }

    }
}
