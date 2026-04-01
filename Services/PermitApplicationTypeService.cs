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
    public class PermitApplicationTypeService : IPermitApplicationTypeService
    {
        private readonly IPermitApplicationTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ApplicationDbContext _context;

        public PermitApplicationTypeService(
            IPermitApplicationTypeRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            ApplicationDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _context = context;
        }

        public async Task<IEnumerable<PermitApplicationType>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PermitApplicationType?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<PermitApplicationType> CreateAsync(CreatePermitApplicationTypeDto dto)
        {
            var permitApplicationType = _mapper.Map<PermitApplicationType>(dto);

            permitApplicationType.CreatedAt = DateTime.UtcNow;
            permitApplicationType.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(permitApplicationType);
            await _repository.SaveChangesAsync();

            return permitApplicationType;
        }

        public async Task<bool> UpdateAsync(int id, UpdatePermitApplicationTypeDto dto)
        {
            var permitApplicationType = await _repository.GetByIdAsync(id);
            if (permitApplicationType == null)
                return false;

            _mapper.Map(dto, permitApplicationType);

            permitApplicationType.UpdatedAt = DateTime.UtcNow;
            permitApplicationType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(permitApplicationType);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var permitApplicationType = await _repository.GetByIdAsync(id);
            if (permitApplicationType == null)
                return false;

            var hasReferences = await _context.BuildingPermits.AnyAsync(x => x.PermitAppTypeId == id);
            if (hasReferences)
                throw new InvalidOperationException("This type of application is already referenced by existing applications. Deactivate it instead of deleting it.");

            permitApplicationType.IsDeleted = true;
            permitApplicationType.UpdatedAt = DateTime.UtcNow;
            permitApplicationType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(permitApplicationType);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var permitApplicationType = await _repository.GetByIdIncludingDeletedAsync(id);
            if (permitApplicationType == null || !permitApplicationType.IsDeleted)
                return false;

            permitApplicationType.IsDeleted = false;
            permitApplicationType.UpdatedAt = DateTime.UtcNow;
            permitApplicationType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(permitApplicationType);
            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<PermitApplicationType>> GetByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination)
        {
            return await _repository.GetByNameAsync(permitAppTypeDesc, pagination);
        }

        public async Task<PagedResult<PermitApplicationType>> FilterByNameAsync(
            string permitAppTypeDesc,
            PaginationParams pagination)
        {
            return await _repository.FilterByNameAsync(permitAppTypeDesc, pagination);
        }
    }
}
