using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class ApplicantTypeSeeder : BaseSeeder
    {
        public override int Order => 50;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<ApplicantType>().HasData(
                new ApplicantType { Id = 1, ApplicantTypeDesc = "Individual",            CreatedBy = createdBy, CreatedAt = now },
                new ApplicantType { Id = 2, ApplicantTypeDesc = "Company/Organization",  CreatedBy = createdBy, CreatedAt = now }
            );
        }
    }
}
