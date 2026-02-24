using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class OwnershipTypeService : IOwnershipTypeService
    {
        private readonly IOwnershipTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public OwnershipTypeService(
            IOwnershipTypeRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<OwnershipType>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<OwnershipType?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OwnershipType> CreateAsync(CreateOwnershipTypeDto dto)
        {
            var ownershipType = _mapper.Map<OwnershipType>(dto);

            ownershipType.CreatedAt = DateTime.UtcNow;
            ownershipType.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(ownershipType);
            await _repository.SaveChangesAsync();

            return ownershipType;
        }

        public async Task<bool> UpdateAsync(int id, UpdateOwnershipTypeDto dto)
        {
            var ownershipType = await _repository.GetByIdAsync(id);
            if (ownershipType == null)
                return false;

            _mapper.Map(dto, ownershipType);

            ownershipType.UpdatedAt = DateTime.UtcNow;
            ownershipType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(ownershipType);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var ownershipType = await _repository.GetByIdAsync(id);
            if (ownershipType == null)
                return false;

            ownershipType.IsDeleted = true;
            ownershipType.UpdatedAt = DateTime.UtcNow;
            ownershipType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(ownershipType);
            return await _repository.SaveChangesAsync();
        }
        public async Task<bool> RestoreAsync(int id)
        {
            var ownershipType = await _repository.GetByIdIncludingDeletedAsync(id);
            if (ownershipType == null || !ownershipType.IsDeleted)
                return false;

            ownershipType.IsDeleted = false;
            ownershipType.UpdatedAt = DateTime.UtcNow;
            ownershipType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(ownershipType);
            return await _repository.SaveChangesAsync();
        }
        public async Task<IEnumerable<OwnershipType>> GetByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination)
        {
            return await _repository.GetByNameAsync(ownershipTypeDesc, pagination);
        }
        public async Task<PagedResult<OwnershipType>> FilterByNameAsync(
            string ownershipTypeDesc,
            PaginationParams pagination)
        {
            return await _repository.FilterByNameAsync(ownershipTypeDesc, pagination);
        }
    }
}
