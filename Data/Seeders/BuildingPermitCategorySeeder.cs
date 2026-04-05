using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class BuildingPermitCategorySeeder : BaseSeeder
    {
        public override int Order => 15;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<BuildingPermitCategory>().HasData(
                new BuildingPermitCategory
                {
                    Id = 1,
                    CategoryName = "Simple",
                    Description = "Basic residential or small-scale projects",
                    CreatedBy = createdBy,
                    CreatedAt = now
                },
                new BuildingPermitCategory
                {
                    Id = 2,
                    CategoryName = "Complex",
                    Description = "Mid-sized or multi-level construction",
                    CreatedBy = createdBy,
                    CreatedAt = now
                },
                new BuildingPermitCategory
                {
                    Id = 3,
                    CategoryName = "Highly Technical",
                    Description = "Large-scale or specialized structures",
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );
        }
    }
}
