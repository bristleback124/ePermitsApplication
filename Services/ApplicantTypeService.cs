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
    public class ApplicantTypeService : IApplicantTypeService
    {
        private readonly IApplicantTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ApplicationDbContext _context;

        public ApplicantTypeService(
            IApplicantTypeRepository repository,
            IMapper mapper,
            ICurrentUserService currentUser,
            ApplicationDbContext context)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUser = currentUser;
            _context = context;
        }

        public async Task<IEnumerable<ApplicantType>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ApplicantType?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ApplicantType> CreateAsync(CreateApplicantTypeDto dto)
        {
            var applicantType = _mapper.Map<ApplicantType>(dto);

            applicantType.CreatedAt = DateTime.UtcNow;
            applicantType.CreatedBy = _currentUser.UserName ?? "System";

            await _repository.AddAsync(applicantType);
            await _repository.SaveChangesAsync();

            return applicantType;
        }

        public async Task<bool> UpdateAsync(int id, UpdateApplicantTypeDto dto)
        {
            var applicantType = await _repository.GetByIdAsync(id);
            if (applicantType == null)
                return false;

            _mapper.Map(dto, applicantType);

            applicantType.UpdatedAt = DateTime.UtcNow;
            applicantType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(applicantType);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var applicantType = await _repository.GetByIdAsync(id);
            if (applicantType == null)
                return false;

            var hasReferences = await _context.BuildingPermitAppInfos.AnyAsync(x => x.ApplicantTypeId == id)
                || await _context.CoOApps.AnyAsync(x => x.ApplicantTypeId == id);
            if (hasReferences)
                throw new InvalidOperationException("This applicant type is already referenced by existing applications. Deactivate it instead of deleting it.");

            applicantType.IsDeleted = true;
            applicantType.UpdatedAt = DateTime.UtcNow;
            applicantType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(applicantType);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> RestoreAsync(int id)
        {
            var applicantType = await _repository.GetByIdIncludingDeletedAsync(id);
            if (applicantType == null || !applicantType.IsDeleted)
                return false;

            applicantType.IsDeleted = false;
            applicantType.UpdatedAt = DateTime.UtcNow;
            applicantType.UpdatedBy = _currentUser.UserName ?? "System";

            _repository.Update(applicantType);
            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ApplicantType>> GetByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination)
        {
            return await _repository.GetByNameAsync(applicantTypeDesc, pagination);
        }

        public async Task<PagedResult<ApplicantType>> FilterByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination)
        {
            return await _repository.FilterByNameAsync(applicantTypeDesc, pagination);
        }
    }
}
