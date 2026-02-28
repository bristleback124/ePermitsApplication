using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class DepartmentSeeder : BaseSeeder
    {
        public override int Order => 5;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    DepartmentCode = "OBO",
                    DepartmentName = "Office of the Building Official",
                    CreatedBy = createdBy,
                    CreatedAt = now
                },
                new Department
                {
                    Id = 2,
                    DepartmentCode = "CPDO",
                    DepartmentName = "City Planning Development Office",
                    CreatedBy = createdBy,
                    CreatedAt = now
                },
                new Department
                {
                    Id = 3,
                    DepartmentCode = "BFP",
                    DepartmentName = "Bureau of Fire Protection",
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );
        }
    }
}
