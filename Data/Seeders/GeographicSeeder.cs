using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class GeographicSeeder : BaseSeeder
    {
        public override int Order => 1;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            // Province
            modelBuilder.Entity<Province>().HasData(
                new Province
                {
                    Id = 1,
                    ProvinceName = "Cebu",
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );

            // LGU
            modelBuilder.Entity<LGU>().HasData(
                new LGU
                {
                    Id = 1,
                    LGUName = "Consolacion",
                    ProvinceId = 1,
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );

            // Barangays of Consolacion
            var barangayNames = new[]
            {
                "Cabangahan", "Cansaga", "Casili", "Danglag", "Garing",
                "Jugan", "Lamac", "Lanipga", "Nangka", "Panas",
                "Pitogo", "Poblacion East", "Poblacion Occidental", "Poblacion Oriental", "Pulpogan",
                "Sacsac", "Tayud", "Tilhaong", "Tolotolo", "Tugbongan"
            };

            var barangays = barangayNames.Select((name, index) => new Barangay
            {
                Id = index + 1,
                BarangayName = name,
                LGUId = 1,
                CreatedBy = createdBy,
                CreatedAt = now
            }).ToArray();

            modelBuilder.Entity<Barangay>().HasData(barangays);
        }
    }
}
