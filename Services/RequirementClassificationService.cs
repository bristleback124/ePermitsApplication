using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class RequirementClassificationService : IRequirementClassificationService
    {
        private readonly IRequirementClassificationRepository _repository;
        private readonly IRequirementCategoryRepository _reqCatRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public RequirementClassificationService(
            IRequirementClassificationRepository repository,
            IRequirementCategoryRepository reqCatRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _reqCatRepository = reqCatRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<RequirementClassification>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RequirementClassification?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<RequirementClassification> CreateAsync(CreateRequirementClassificationDto dto)
        {
            var reqClass = _mapper.Map<RequirementClassification>(dto);

            reqClass.CreatedAt = DateTime.UtcNow;
            reqClass.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(reqClass);
            await _repository.SaveChangesAsync();

            return reqClass;
        }

        public async Task<bool> UpdateAsync(int id, UpdateRequirementClassificationDto dto)
        {
            var reqClass = await _repository.GetByIdAsync(id);
            if (reqClass == null)
                return false;

            _mapper.Map(dto, reqClass);

            reqClass.UpdatedAt = DateTime.UtcNow;
            reqClass.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(reqClass);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var reqClass = await _repository.GetByIdAsync(id);
            if (reqClass == null)
                return false;

            reqClass.IsDeleted = true;
            reqClass.UpdatedAt = DateTime.UtcNow;
            reqClass.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(reqClass);
            return await _repository.SaveChangesAsync();
        }
        public async Task<bool> RestoreAsync(int id)
        {
            var reqClass = await _repository.GetByIdIncludingDeletedAsync(id);
            if (reqClass == null || !reqClass.IsDeleted)
                return false;

            reqClass.IsDeleted = false;
            reqClass.UpdatedAt = DateTime.UtcNow;
            reqClass.UpdatedBy = _currentUser.UserName ?? "System";

           // Cascade restore RequirementCategory
           var reqCats = await _reqCatRepository.GetByReqClassIncludingDeletedAsync(id);
            foreach (var reqCat in reqCats.Where(r => r.IsDeleted))
            {
                reqCat.IsDeleted = false;
                reqCat.UpdatedAt = DateTime.UtcNow;
                reqCat.UpdatedBy = _currentUser.UserName ?? "System";
            }

            _repository.Update(reqClass);
            return await _repository.SaveChangesAsync();
        }
        public async Task<IEnumerable<RequirementClassification>> GetByNameAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            return await _repository.GetByNameAsync(reqClassDesc, pagination);
        }
        public async Task<PagedResult<RequirementClassification>> FilterByNameAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            return await _repository.FilterByNameAsync(reqClassDesc, pagination);
        }
    }
}
