using ePermitsApp.DTOs;
using ePermitsApp.Helpers;
using ePermitsApp.Repositories.Interfaces;
using ePermitsApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ePermitsApp.Data;

namespace ePermitsApp.Services;

public class AdminEmailNotificationConfigService : IAdminEmailNotificationConfigService
{
    private readonly IAdminEmailNotificationConfigRepository _repository;
    private readonly ApplicationDbContext _context;

    private static readonly HashSet<string> ValidApplicationTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit,
        ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy
    };

    public AdminEmailNotificationConfigService(
        IAdminEmailNotificationConfigRepository repository,
        ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<AdminEmailNotificationConfigResponseDto> GetConfigAsync()
    {
        var allConfigs = await _repository.GetAllAsync();
        var allUsers = await GetAllUsersWithProfilesAsync();

        var grouped = new List<AdminEmailNotificationConfigGroupDto>
        {
            new()
            {
                ApplicationType = ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit,
                ApplicationTypeLabel = "Building Permit",
                UserIds = allConfigs
                    .Where(c => c.ApplicationType == ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit)
                    .Select(c => c.UserId)
                    .ToList()
            },
            new()
            {
                ApplicationType = ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy,
                ApplicationTypeLabel = "Certificate of Occupancy",
                UserIds = allConfigs
                    .Where(c => c.ApplicationType == ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy)
                    .Select(c => c.UserId)
                    .ToList()
            }
        };

        return new AdminEmailNotificationConfigResponseDto
        {
            Configs = grouped,
            AvailableAdmins = allUsers.Select(u => new AdminUserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.UserProfile != null
                    ? $"{u.UserProfile.FirstName} {u.UserProfile.LastName}".Trim()
                    : u.Username,
                Email = u.UserProfile?.Email ?? string.Empty
            }).ToList()
        };
    }

    public async Task UpdateConfigAsync(UpdateAdminEmailNotificationConfigDto dto)
    {
        if (!ValidApplicationTypes.Contains(dto.ApplicationType))
        {
            throw new ArgumentException($"Invalid application type: {dto.ApplicationType}");
        }

        // Validate that all user IDs exist
        var allUsers = await GetAllUsersWithProfilesAsync();
        var validIds = allUsers.Select(u => u.Id).ToHashSet();
        var invalidIds = dto.UserIds.Where(id => !validIds.Contains(id)).ToList();
        if (invalidIds.Any())
        {
            throw new ArgumentException($"Invalid user IDs: {string.Join(", ", invalidIds)}");
        }

        await _repository.ReplaceForApplicationTypeAsync(dto.ApplicationType, dto.UserIds);
    }

    public async Task<List<string>> GetRecipientEmailsAsync(string applicationType)
    {
        var configs = await _repository.GetByApplicationTypeAsync(applicationType);

        if (configs.Any())
        {
            // Specific admins configured — return their emails
            return configs
                .Where(c => c.User?.UserProfile != null && !string.IsNullOrWhiteSpace(c.User.UserProfile.Email))
                .Select(c => c.User!.UserProfile!.Email)
                .ToList();
        }

        // Default: all admin emails
        var adminUsers = await GetAdminUsersAsync();
        return adminUsers
            .Where(u => u.UserProfile != null && !string.IsNullOrWhiteSpace(u.UserProfile.Email))
            .Select(u => u.UserProfile!.Email)
            .ToList();
    }

    private static readonly HashSet<string> SelectableRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin", "superadmin", "sysadmin", "user", "encoder", "initial-reviewer", "technical-reviewer", "fee-assessor", "final-reviewer", "final-approver"
    };

    private async Task<List<ePermits.Models.User>> GetAllUsersWithProfilesAsync()
    {
        return await _context.Users
            .Include(u => u.UserRole)
            .Include(u => u.UserProfile)
            .Where(u => u.UserProfile != null
                && u.UserRole != null
                && SelectableRoles.Contains(u.UserRole.UserRoleDesc))
            .ToListAsync();
    }

    private async Task<List<ePermits.Models.User>> GetAdminUsersAsync()
    {
        return await _context.Users
            .Include(u => u.UserRole)
            .Include(u => u.UserProfile)
            .Where(u => u.UserRole != null && (u.UserRole.UserRoleDesc == "admin" || u.UserRole.UserRoleDesc == "superadmin" || u.UserRole.UserRoleDesc == "sysadmin"))
            .ToListAsync();
    }
}
