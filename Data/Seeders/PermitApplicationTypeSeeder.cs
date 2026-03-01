using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class PermitApplicationTypeSeeder : BaseSeeder
    {
        public override int Order => 20;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<PermitApplicationType>().HasData(
                new PermitApplicationType { Id = 1, PermitAppTypeDesc = "New Construction", CreatedBy = createdBy, CreatedAt = now },
                new PermitApplicationType { Id = 2, PermitAppTypeDesc = "Renovation",       CreatedBy = createdBy, CreatedAt = now },
                new PermitApplicationType { Id = 3, PermitAppTypeDesc = "Addition",          CreatedBy = createdBy, CreatedAt = now },
                new PermitApplicationType { Id = 4, PermitAppTypeDesc = "Repair",            CreatedBy = createdBy, CreatedAt = now },
                new PermitApplicationType { Id = 5, PermitAppTypeDesc = "Demolition",        CreatedBy = createdBy, CreatedAt = now },
                new PermitApplicationType { Id = 6, PermitAppTypeDesc = "Fencing",           CreatedBy = createdBy, CreatedAt = now },
                new PermitApplicationType { Id = 7, PermitAppTypeDesc = "Others",            CreatedBy = createdBy, CreatedAt = now }
            );
        }
    }
}
