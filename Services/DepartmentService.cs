using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public DepartmentService(
            IDepartmentRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Department> CreateAsync(CreateDepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);

            department.CreatedAt = DateTime.UtcNow;
            department.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return await _repository.GetByIdAsync(department.Id) ?? department;
        }

        public async Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await _repository.GetByIdAsync(id);
            if (department == null)
                return false;

            _mapper.Map(dto, department);

            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(department);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var department = await _repository.GetByIdAsync(id);
            if (department == null)
                return false;

            department.IsDeleted = true;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(department);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var department = await _repository.GetByIdIncludingDeletedAsync(id);
            if (department == null || !department.IsDeleted)
                return false;

            department.IsDeleted = false;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(department);
            return await _repository.SaveChangesAsync();
        }

        public async Task<PagedResult<Department>> FilterAsync(
            string? departmentName,
            string? departmentCode,
            PaginationParams pagination)
        {
            return await _repository.FilterAsync(
                departmentName, departmentCode, pagination);
        }
    }
}
