using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Repositories.Interfaces
{
    public interface IApplicantTypeRepository
    {
        Task<IEnumerable<ApplicantType>> GetAllAsync();
        Task<ApplicantType?> GetByIdAsync(int id);
        Task AddAsync(ApplicantType applicantType);
        void Update(ApplicantType applicantType);
        Task<bool> SaveChangesAsync();
        Task<ApplicantType?> GetByIdIncludingDeletedAsync(int id);
        Task<IEnumerable<ApplicantType>> GetByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination);
        Task<PagedResult<ApplicantType>> FilterByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination);
    }
}
