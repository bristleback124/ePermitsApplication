using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class BarangayService : IBarangayService
    {
        private readonly IBarangayRepository _repository;
        private readonly ILGURepository _lguRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public BarangayService(
            IBarangayRepository repository,
            ILGURepository lguRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _lguRepository = lguRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Barangay>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Barangay?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Barangay> CreateAsync(CreateBarangayDto dto)
        {
            var lgu = await _lguRepository.GetByIdAsync(dto.LGUId);
            if (lgu == null)
                throw new Exception("LGU not found");

            var barangay = _mapper.Map<Barangay>(dto);

            barangay.CreatedAt = DateTime.UtcNow;
            barangay.CreatedBy = _currentUser.UserName ?? "System";

            //var barangay = new Barangay
            //{
            //    BarangayName = dto.BarangayName,
            //    LGUId = dto.LGUId,
            //    CreatedAt = DateTime.UtcNow,
            //    CreatedBy = _currentUser.UserName!
            //};

            await _repository.AddAsync(barangay);
            await _repository.SaveChangesAsync();

            return barangay;
        }

        public async Task<bool> UpdateAsync(int id, UpdateBarangayDto dto)
        {
            var barangay = await _repository.GetByIdAsync(id);
            if (barangay == null) 
                return false;

            var lguExists = await _lguRepository.GetByIdAsync(dto.LGUId);
            if (lguExists == null)
                throw new Exception("LGU not found");

            _mapper.Map(dto, barangay);

            barangay.UpdatedAt = DateTime.UtcNow;
            barangay.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(barangay);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var barangay = await _repository.GetByIdAsync(id);
            if (barangay == null) 
                return false;

            barangay.IsDeleted = true;
            barangay.UpdatedAt = DateTime.UtcNow;
            barangay.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(barangay);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var barangay = await _repository.GetByIdIncludingDeletedAsync(id);
            if (barangay == null || !barangay.IsDeleted) 
                return false;

            // Prevent restore if parent LGU or Province is deleted
            if (barangay.LGU.IsDeleted || barangay.LGU.Province.IsDeleted)
                return false;

            barangay.IsDeleted = false;
            barangay.UpdatedAt = DateTime.UtcNow;
            barangay.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(barangay);
            return await _repository.SaveChangesAsync();
        }

        public async Task<PagedResult<Barangay>> FilterAsync(
            string? barangayName,
            int? lguId,
            string? provinceName,
            PaginationParams pagination)
        {
            return await _repository.FilterAsync(
                barangayName, lguId, provinceName, pagination);
        }
    }
}
