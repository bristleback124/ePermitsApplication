using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class OwnershipTypeSeeder : BaseSeeder
    {
        public override int Order => 40;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<OwnershipType>().HasData(
                new OwnershipType { Id = 1, OwnershipTypeDesc = "Owner",                    CreatedBy = createdBy, CreatedAt = now },
                new OwnershipType { Id = 2, OwnershipTypeDesc = "Lessee",                   CreatedBy = createdBy, CreatedAt = now },
                new OwnershipType { Id = 3, OwnershipTypeDesc = "Authorized Representative", CreatedBy = createdBy, CreatedAt = now }
            );
        }
    }
}
