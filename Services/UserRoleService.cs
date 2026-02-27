using AutoMapper;
using ePermits.Data;
using ePermits.Models;
using ePermitsApp.DTOs;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public UserRoleService(
            IUserRoleRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<UserRole>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<UserRole?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<UserRole?> GetByDescriptionAsync(string description)
        {
            return await _repository.GetByDescriptionAsync(description);
        }

        public async Task<UserRole> CreateAsync(CreateUserRoleDto dto)
        {
            var userRole = _mapper.Map<UserRole>(dto);
            userRole.CreatedAt = DateTime.UtcNow;
            userRole.CreatedBy = _currentUser.UserName ?? "System";

            return await _repository.CreateAsync(userRole);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserRoleDto dto)
        {
            var userRole = await _repository.GetByIdAsync(id);
            if (userRole == null)
            {
                return false;
            }

            _mapper.Map(dto, userRole);
            userRole.UpdatedAt = DateTime.UtcNow;
            userRole.UpdatedBy = _currentUser.UserName ?? "System";

            await _repository.UpdateAsync(userRole);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
