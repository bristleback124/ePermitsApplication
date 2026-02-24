using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _repository;
        private readonly ILGURepository _lguRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public ProvinceService(
            IProvinceRepository repository,
            ILGURepository lguRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _lguRepository = lguRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Province>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Province?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Province> CreateAsync(CreateProvinceDto dto)
        {
            var province = _mapper.Map<Province>(dto);

            province.CreatedAt = DateTime.UtcNow;
            province.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(province);
            await _repository.SaveChangesAsync();

            return province;
        }

        public async Task<bool> UpdateAsync(int id, UpdateProvinceDto dto)
        {
            var province = await _repository.GetByIdAsync(id);
            if (province == null)
                return false;

            _mapper.Map(dto, province);

            province.UpdatedAt = DateTime.UtcNow;
            province.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(province);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var province = await _repository.GetByIdAsync(id);
            if (province == null)
                return false;

            province.IsDeleted = true;
            province.UpdatedAt = DateTime.UtcNow;
            province.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(province);
            return await _repository.SaveChangesAsync();
        }
        public async Task<bool> RestoreAsync(int id)
        {
            var province = await _repository.GetByIdIncludingDeletedAsync(id);
            if (province == null || !province.IsDeleted)
                return false;

            province.IsDeleted = false;
            province.UpdatedAt = DateTime.UtcNow;
            province.UpdatedBy = _currentUser.UserName ?? "System";

            // Cascade restore LGUs
            var lgus = await _lguRepository.GetByProvinceIncludingDeletedAsync(id);
            foreach (var lgu in lgus.Where(l => l.IsDeleted))
            {
                lgu.IsDeleted = false;
                lgu.UpdatedAt = DateTime.UtcNow;
                lgu.UpdatedBy = _currentUser.UserName ?? "System";
            }

            _repository.Update(province);
            return await _repository.SaveChangesAsync();
        }
        public async Task<IEnumerable<Province>> GetByNameAsync(
            string provinceName,
            PaginationParams pagination)
        {
            return await _repository.GetByNameAsync(provinceName, pagination);
        }
        public async Task<PagedResult<Province>> FilterByNameAsync(
            string provinceName,
            PaginationParams pagination)
        {
            return await _repository.FilterByNameAsync(provinceName, pagination);
        }
    }
}
