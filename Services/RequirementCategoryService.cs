using AutoMapper;
using ePermitsApp.DTOs;
using ePermitsApp.Entities;
using ePermitsApp.Repositories;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;

namespace ePermitsApp.Services
{
    public class RequirementCategoryService : IRequirementCategoryService
    {
        private readonly IRequirementCategoryRepository _repository;
        private readonly IRequirementClassificationRepository _reqClassRepository;
        private readonly IRequirementRepository _reqRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public RequirementCategoryService(
            IRequirementCategoryRepository repository,
            IRequirementClassificationRepository reqClassRepository,
            IRequirementRepository reqRepository,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _reqClassRepository = reqClassRepository;
            _reqRepository = reqRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<RequirementCategory>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RequirementCategory?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<RequirementCategory> CreateAsync(CreateRequirementCategoryDto dto)
        {
            var reqClass = await _reqClassRepository.GetByIdAsync(dto.ReqClassId);
            if (reqClass == null)
                throw new Exception("Requirement Classification not found");

            var reqCat = _mapper.Map<RequirementCategory>(dto);

            reqCat.CreatedAt = DateTime.UtcNow;
            reqCat.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(reqCat);
            await _repository.SaveChangesAsync();

            return reqCat;
        }

        public async Task<bool> UpdateAsync(int id, UpdateRequirementCategoryDto dto)
        {
            var reqCat = await _repository.GetByIdAsync(id);
            if (reqCat == null)
                return false;

            var reqClassExists = await _reqClassRepository.GetByIdAsync(dto.ReqClassId);
            if (reqClassExists == null)
                throw new Exception("Requirement Classification not found");

            _mapper.Map(dto, reqCat);

            reqCat.UpdatedAt = DateTime.UtcNow;
            reqCat.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(reqCat);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var reqCat = await _repository.GetByIdAsync(id);
            if (reqCat == null || reqCat.IsDeleted)
                return false;

            // Block delete if Requirement Classification already deleted (optional but consistent)
            if (reqCat.RequirementClassification.IsDeleted)
                return false;

            var now = DateTime.UtcNow;
            var user = _currentUser.UserName ?? "System";

            // Soft delete Requirement Category
            reqCat.IsDeleted = true;
            reqCat.UpdatedAt = now;
            reqCat.UpdatedBy = user;

            _repository.Update(reqCat);

            // Soft delete Requirement
            var reqs = await _reqRepository.GetByReqCatAsync(id);
            foreach (var req in reqs)
            {
                req.IsDeleted = true;
                req.UpdatedAt = now;
                req.UpdatedBy = user;

                _reqRepository.Update(req);
            }

            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var reqCat = await _repository.GetByIdIncludingDeletedAsync(id);
            if (reqCat == null || !reqCat.IsDeleted)
                return false;

            // Prevent restore if Requirement Classification is deleted
            var reqClass = await _reqClassRepository
                .GetByIdIncludingDeletedAsync(reqCat.ReqClassId);

            if (reqClass == null || reqClass.IsDeleted)
                throw new InvalidOperationException(
                    "Cannot restore Requirement Category while parent Requirement Classification is deleted.");

            var now = DateTime.UtcNow;
            var user = _currentUser.UserName ?? "System";

            // Restore Requirement Category
            reqCat.IsDeleted = false;
            reqCat.UpdatedAt = now;
            reqCat.UpdatedBy = user;

            _repository.Update(reqCat);

            // Restore Requirement
            var deletedReqs = await _reqRepository.GetDeletedByReqCatAsync(id);
            foreach (var req in deletedReqs)
            {
                req.IsDeleted = false;
                req.UpdatedAt = now;
                req.UpdatedBy = user;

                _reqRepository.Update(req);
            }

            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<RequirementCategory>> GetByReqClassAsync(int reqClassId)
        {
            return await _repository.GetByReqClassAsync(reqClassId);
        }

        public async Task<IEnumerable<RequirementCategory>> GetByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            return await _repository.GetByReqClassDescAsync(reqClassDesc, pagination);
        }
        public async Task<PagedResult<RequirementCategory>> FilterByReqClassDescAsync(
            string reqClassDesc,
            PaginationParams pagination)
        {
            if (string.IsNullOrWhiteSpace(reqClassDesc))
            {
                // Optional: return empty result instead of querying everything
                return new PagedResult<RequirementCategory>
                {
                    Items = Enumerable.Empty<RequirementCategory>(),
                    TotalCount = 0,
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };
            }

            return await _repository.FilterByReqClassDescAsync(
                reqClassDesc,
                pagination);
        }
    }
}
