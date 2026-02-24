using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ePermitsApp.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly ILGURepository _lguRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public DepartmentService(
            IDepartmentRepository repository,
            ILGURepository lguRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _lguRepository = lguRepository;
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
            var lgu = await _lguRepository.GetByIdAsync(dto.LGUId);
            if (lgu == null)
                throw new Exception("LGU not found");

            var department = _mapper.Map<Department>(dto);

            department.CreatedAt = DateTime.UtcNow;
            department.CreatedBy = _currentUser.UserName ?? "System";

            //var department = new Department
            //{
            //    DepartmentCode = dto.DepartmentCode,
            //    DepartmentName = dto.DepartmentName,
            //    LGUId = dto.LGUId,
            //    CreatedAt = DateTime.UtcNow,
            //    CreatedBy = _currentUser.UserName!
            //};

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return department;
        }

        public async Task<bool> UpdateAsync(int id, UpdateDepartmentDto dto)
        {            
            var department = await _repository.GetByIdAsync(id);
            if (department == null) 
                return false;

            var lguExists = await _lguRepository.GetByIdAsync(dto.LGUId);
            if (lguExists == null)
                throw new Exception("LGU not found");

            _mapper.Map(dto, department);

            //department.DepartmentCode = dto.DepartmentCode;
            //department.DepartmentName = dto.DepartmentName;
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

            // Prevent restore if parent LGU or Province is deleted
            if (department.LGU.IsDeleted || department.LGU.Province.IsDeleted)
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
            int? lguId,
            string? provinceName,
            PaginationParams pagination)
        {
            return await _repository.FilterAsync(
                departmentName, departmentCode, lguId, provinceName, pagination);
        }        
    }
}
