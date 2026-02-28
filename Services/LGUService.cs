using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class LGUService : ILGUService
    {
        private readonly ILGURepository _repository;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IBarangayRepository _barangayRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public LGUService(
            ILGURepository repository,
            IProvinceRepository provinceRepository,
            IBarangayRepository barangayRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _provinceRepository = provinceRepository;
            _barangayRepository = barangayRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<LGU>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<LGU?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<LGU> CreateAsync(CreateLGUDto dto)
        {
            var province = await _provinceRepository.GetByIdAsync(dto.ProvinceId);
            if (province == null)
                throw new Exception("Province not found");

            var lgu = _mapper.Map<LGU>(dto);

            lgu.CreatedAt = DateTime.UtcNow;
            lgu.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(lgu);
            await _repository.SaveChangesAsync();

            return lgu;
        }

        public async Task<bool> UpdateAsync(int id, UpdateLGUDto dto)
        {
            var lgu = await _repository.GetByIdAsync(id);
            if (lgu == null)
                return false;

            var provinceExists = await _provinceRepository.GetByIdAsync(dto.ProvinceId);
            if (provinceExists == null)
                throw new Exception("Province not found");

            _mapper.Map(dto, lgu);

            lgu.UpdatedAt = DateTime.UtcNow;
            lgu.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(lgu);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var lgu = await _repository.GetByIdAsync(id);
            if (lgu == null || lgu.IsDeleted)
                return false;

            // Block delete if Province already deleted (optional but consistent)
            if (lgu.Province.IsDeleted)
                return false;

            var now = DateTime.UtcNow;
            var user = _currentUser.UserName ?? "System";

            // Soft delete LGU
            lgu.IsDeleted = true;
            lgu.UpdatedAt = now;
            lgu.UpdatedBy = user;

            _repository.Update(lgu);
                        
            // Soft delete Barangays
            var barangays = await _barangayRepository.GetByLGUAsync(id);
            foreach (var barangay in barangays)
            {
                barangay.IsDeleted = true;
                barangay.UpdatedAt = now;
                barangay.UpdatedBy = user;

                _barangayRepository.Update(barangay);
            }

            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var lgu = await _repository.GetByIdIncludingDeletedAsync(id);
            if (lgu == null || !lgu.IsDeleted)
                return false;

            // Prevent restore if Province is deleted
            var province = await _provinceRepository
                .GetByIdIncludingDeletedAsync(lgu.ProvinceId);

            if (province == null || province.IsDeleted)
                throw new InvalidOperationException(
                    "Cannot restore LGU while parent Province is deleted.");

            var now = DateTime.UtcNow;
            var user = _currentUser.UserName ?? "System";

            // Restore LGU
            lgu.IsDeleted = false;
            lgu.UpdatedAt = now;
            lgu.UpdatedBy = user;

            _repository.Update(lgu);

            // Restore Barangays
            var deletedBarangays = await _barangayRepository.GetDeletedByLGUAsync(id);
            foreach (var barangay in deletedBarangays)
            {
                barangay.IsDeleted = false;
                barangay.UpdatedAt = now;
                barangay.UpdatedBy = user;

                _barangayRepository.Update(barangay);
            }

            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<LGU>> GetByProvinceAsync(int provinceId)
        {
            return await _repository.GetByProvinceAsync(provinceId);
        }

        public async Task<IEnumerable<LGU>> GetByProvinceNameAsync(
            string provinceName, 
            PaginationParams pagination)
        {
            return await _repository.GetByProvinceNameAsync(provinceName, pagination);
        }
        public async Task<PagedResult<LGU>> FilterByProvinceNameAsync(
            string provinceName,
            PaginationParams pagination)
        {
            if (string.IsNullOrWhiteSpace(provinceName))
            {
                // Optional: return empty result instead of querying everything
                return new PagedResult<LGU>
                {
                    Items = Enumerable.Empty<LGU>(),
                    TotalCount = 0,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            }

            return await _repository.FilterByProvinceNameAsync(
                provinceName,
                pagination);
        }
    }
}
