using AutoMapper;
using ePermitsApp.Data;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Services
{
    public class OccupancyNatureService : IOccupancyNatureService
    {
        private readonly IOccupancyNatureRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ApplicationDbContext _context;

        public OccupancyNatureService(
            IOccupancyNatureRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            ApplicationDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _context = context;
        }

        public async Task<IEnumerable<OccupancyNature>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<OccupancyNature?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OccupancyNature> CreateAsync(CreateOccupancyNatureDto dto)
        {
            var occupancyNature = _mapper.Map<OccupancyNature>(dto);

            occupancyNature.CreatedAt = DateTime.UtcNow;
            occupancyNature.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(occupancyNature);
            await _repository.SaveChangesAsync();

            return occupancyNature;
        }

        public async Task<bool> UpdateAsync(int id, UpdateOccupancyNatureDto dto)
        {
            var occupancyNature = await _repository.GetByIdAsync(id);
            if (occupancyNature == null)
                return false;

            _mapper.Map(dto, occupancyNature);

            occupancyNature.UpdatedAt = DateTime.UtcNow;
            occupancyNature.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(occupancyNature);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var occupancyNature = await _repository.GetByIdAsync(id);
            if (occupancyNature == null)
                return false;

            var hasReferences = await _context.BuildingPermits.AnyAsync(x => x.OccupancyNatureId == id)
                || await _context.CoOApps.AnyAsync(x => x.OccupancyNatureId == id);
            if (hasReferences)
                throw new InvalidOperationException("This occupancy classification is already referenced by existing applications. Deactivate it instead of deleting it.");

            occupancyNature.IsDeleted = true;
            occupancyNature.UpdatedAt = DateTime.UtcNow;
            occupancyNature.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(occupancyNature);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var occupancyNature = await _repository.GetByIdIncludingDeletedAsync(id);
            if (occupancyNature == null || !occupancyNature.IsDeleted)
                return false;

            occupancyNature.IsDeleted = false;
            occupancyNature.UpdatedAt = DateTime.UtcNow;
            occupancyNature.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(occupancyNature);
            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<OccupancyNature>> GetByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination)
        {
            return await _repository.GetByNameAsync(occupancyNatureDesc, pagination);
        }

        public async Task<PagedResult<OccupancyNature>> FilterByNameAsync(
            string occupancyNatureDesc,
            PaginationParams pagination)
        {
            return await _repository.FilterByNameAsync(occupancyNatureDesc, pagination);
        }
    }
}
