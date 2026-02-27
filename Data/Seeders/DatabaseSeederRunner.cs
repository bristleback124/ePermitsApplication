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
                new RequirementSeeder(),
                // Add future seeders here, e.g.:
                // new ApplicantTypeSeeder(),
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
