using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public static class DatabaseSeederRunner
    {
        /// <summary>
        /// Instantiates all seeders, orders them, and calls Seed() on each.
        /// Call from ApplicationDbContext.OnModelCreating().
        /// </summary>
        public static void RunSeeders(ModelBuilder modelBuilder)
        {
            var seeders = new List<ISeeder>
            {
                new GeographicSeeder(),
                new DepartmentSeeder(),
                new BuildingPermitCategorySeeder(),
                new RequirementSeeder(),
                new PermitApplicationTypeSeeder(),
                new ProjectClassificationSeeder(),
                new OwnershipTypeSeeder(),
                new OccupancyNatureSeeder(),
                new ApplicantTypeSeeder(),
                new AdminSeeder(),
            }
            .OrderBy(s => s.Order)
            .ToList();

            foreach (var seeder in seeders)
            {
                seeder.Seed(modelBuilder);
            }
        }
    }
}
