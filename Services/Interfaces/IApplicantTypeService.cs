using ePermitsApp.DTOs;
using ePermitsApp.Entities;

namespace ePermitsApp.Services.Interfaces
{
    public interface IApplicantTypeService
    {
        Task<IEnumerable<ApplicantType>> GetAllAsync();
        Task<ApplicantType?> GetByIdAsync(int id);
        Task<ApplicantType> CreateAsync(CreateApplicantTypeDto dto);
        Task<bool> UpdateAsync(int id, UpdateApplicantTypeDto dto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<IEnumerable<ApplicantType>> GetByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination);
        Task<PagedResult<ApplicantType>> FilterByNameAsync(
            string applicantTypeDesc,
            PaginationParams pagination);
    }
}
