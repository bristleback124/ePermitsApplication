using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Extensions;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories
{
    public class ApplicantTypeRepository : IApplicantTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicantTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApplicantType>> GetAllAsync()
        {
            return await _context.ApplicantTypes.AsNoTracking().ToListAsync();
        }

        public async Task<ApplicantType?> GetByIdAsync(int id)
        {
            return await _context.ApplicantTypes.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(ApplicantType applicantType)
        {
            await _context.ApplicantTypes.AddAsync(applicantType);
        }

        public void Update(ApplicantType applicantType)
        {
            _context.ApplicantTypes.Update(applicantType);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<ApplicantType?> GetByIdIncludingDeletedAsync(int id)
        {
            return await _context.ApplicantTypes
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<IEnumerable<ApplicantType>> GetByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination)
        {
            return await _context.ApplicantTypes
                .Where(a => a.ApplicantTypeDesc.ToLower()
                    .Contains(applicantTypeDesc.ToLower()))
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();
        }
        public async Task<PagedResult<ApplicantType>> FilterByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination)
        {
            var query = _context.ApplicantTypes
                .Where(a => a.ApplicantTypeDesc.ToLower()
                    .Contains(applicantTypeDesc.ToLower()));

            var totalCount = await query.CountAsync();

            var items = await query
                .ApplySorting(pagination.OrderBy, pagination.IsDescending)
                .AsNoTracking()
                .Paginate(pagination)
                .ToListAsync();

            return new PagedResult<ApplicantType>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
