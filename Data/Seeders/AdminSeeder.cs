using System.Security.Cryptography;
using System.Text;
using ePermits.Models;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class AdminSeeder : BaseSeeder
    {
        public override int Order => 20;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            var hashedPassword = HashPassword("password123");

            // Seed admin user (UserRoleId 1 = admin, LGUId 1 = Consolacion, no department — admin has access to all)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = hashedPassword,
                    UserRoleId = 1,
                    UserProfileId = 1,
                    LGUId = 1,
                    DepartmentId = null,
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );

            // Seed admin user profile
            modelBuilder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    Id = 1,
                    UserId = 1,
                    FirstName = "System",
                    MiddleName = "",
                    LastName = "Administrator",
                    Email = "admin@epermits.com",
                    MobileNo = "0000000000",
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
