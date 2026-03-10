using ePermitsApp.Data;
using ePermitsApp.Entities;
using ePermitsApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Repositories;

public class AdminEmailNotificationConfigRepository : IAdminEmailNotificationConfigRepository
{
    private readonly ApplicationDbContext _context;

    public AdminEmailNotificationConfigRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminEmailNotificationConfig>> GetByApplicationTypeAsync(string applicationType)
    {
        return await _context.AdminEmailNotificationConfigs
            .Include(c => c.User)
                .ThenInclude(u => u!.UserProfile)
            .Where(c => c.ApplicationType == applicationType)
            .ToListAsync();
    }

    public async Task<List<AdminEmailNotificationConfig>> GetAllAsync()
    {
        return await _context.AdminEmailNotificationConfigs
            .Include(c => c.User)
                .ThenInclude(u => u!.UserProfile)
            .ToListAsync();
    }

    public async Task ReplaceForApplicationTypeAsync(string applicationType, List<int> userIds)
    {
        var existing = await _context.AdminEmailNotificationConfigs
            .Where(c => c.ApplicationType == applicationType)
            .ToListAsync();

        _context.AdminEmailNotificationConfigs.RemoveRange(existing);

        var now = DateTime.UtcNow;
        var newConfigs = userIds.Select(userId => new AdminEmailNotificationConfig
        {
            ApplicationType = applicationType,
            UserId = userId,
            CreatedAt = now
        });

        await _context.AdminEmailNotificationConfigs.AddRangeAsync(newConfigs);
        await _context.SaveChangesAsync();
    }
}
