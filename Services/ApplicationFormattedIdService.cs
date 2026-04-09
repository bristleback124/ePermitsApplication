using ePermitsApp.Data;
using ePermitsApp.Helpers;
using ePermitsApp.Services.Interfaces;
using ePermits.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Services
{
    public class ApplicationFormattedIdService : IApplicationFormattedIdService
    {
        private const int MaxRetries = 5;

        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ApplicationFormattedIdService> _logger;

        public ApplicationFormattedIdService(
            ApplicationDbContext dbContext,
            ILogger<ApplicationFormattedIdService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AssignFormattedIdAsync(Application application, CancellationToken cancellationToken = default)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.Id <= 0)
            {
                throw new InvalidOperationException("Application must be saved before assigning a formatted ID.");
            }

            var prefix = application.Type switch
            {
                ApplicationWorkflowDefinitions.PermitTypes.BuildingPermit => "BP",
                ApplicationWorkflowDefinitions.PermitTypes.CertificateOfOccupancy => "CO",
                _ => throw new InvalidOperationException($"Unsupported application type: {application.Type}")
            };

            var createdAt = application.CreatedAt;
            var year = createdAt.Year;
            var month = createdAt.Month;
            var yy = (year % 100).ToString("D2");
            var mm = month.ToString("D2");

            for (var attempt = 1; attempt <= MaxRetries; attempt++)
            {
                var sequence = await _dbContext.Applications
                    .AsNoTracking()
                    .Where(a =>
                        a.Id != application.Id &&
                        a.Type == application.Type &&
                        a.Status != ApplicationWorkflowDefinitions.OverallStatuses.Draft &&
                        a.CreatedAt.Year == year &&
                        a.CreatedAt.Month == month)
                    .CountAsync(cancellationToken) + 1;

                application.FormattedId = $"{prefix}-01-{yy}-{mm}-{sequence:D3}";

                try
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return;
                }
                catch (DbUpdateException ex) when (IsFormattedIdConflict(ex) && attempt < MaxRetries)
                {
                    _logger.LogWarning(
                        ex,
                        "Formatted ID conflict for application {ApplicationId} on attempt {Attempt}. Retrying.",
                        application.Id,
                        attempt);
                }
            }

            throw new InvalidOperationException(
                $"Failed to assign a unique formatted ID for application {application.Id} after {MaxRetries} attempts.");
        }

        private static bool IsFormattedIdConflict(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException &&
                   (sqlException.Number == 2601 || sqlException.Number == 2627);
        }
    }
}
