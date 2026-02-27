using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class ProjectClassificationSeeder : BaseSeeder
    {
        public override int Order => 30;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<ProjectClassification>().HasData(
                new ProjectClassification { Id = 1, ProjectClassDesc = "Private",    CreatedBy = createdBy, CreatedAt = now },
                new ProjectClassification { Id = 2, ProjectClassDesc = "Government", CreatedBy = createdBy, CreatedAt = now }
            );
        }
    }
}
