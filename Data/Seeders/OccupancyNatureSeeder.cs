using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class OccupancyNatureSeeder : BaseSeeder
    {
        public override int Order => 50;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<OccupancyNature>().HasData(
                new OccupancyNature { Id = 1, OccupancyNatureDesc = "Residential",    CreatedBy = createdBy, CreatedAt = now },
                new OccupancyNature { Id = 2, OccupancyNatureDesc = "Commercial",     CreatedBy = createdBy, CreatedAt = now },
                new OccupancyNature { Id = 3, OccupancyNatureDesc = "Industrial",     CreatedBy = createdBy, CreatedAt = now },
                new OccupancyNature { Id = 4, OccupancyNatureDesc = "Institutional",  CreatedBy = createdBy, CreatedAt = now },
                new OccupancyNature { Id = 5, OccupancyNatureDesc = "Agricultural",   CreatedBy = createdBy, CreatedAt = now },
                new OccupancyNature { Id = 6, OccupancyNatureDesc = "Others",         CreatedBy = createdBy, CreatedAt = now }
            );
        }
    }
}
