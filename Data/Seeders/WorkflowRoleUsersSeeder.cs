using System.Security.Cryptography;
using System.Text;
using ePermits.Models;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class WorkflowRoleUsersSeeder : BaseSeeder
    {
        public override int Order => 25; // After AdminSeeder (20)

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";
            var hashedPassword = HashPassword("password123");

            // Seed users for each workflow role
            // UserRole IDs: 4=encoder, 5=initial-reviewer, 6=fee-assessor,
            //               7=final-reviewer, 8=final-approver, 9=executive, 10=sysadmin,
            //               12=releasing-officer (11 reserved for a parallel feature)

            var users = new[]
            {
                new { Id = 100, Username = "encoder", RoleId = 4, FirstName = "Juan", LastName = "Dela Cruz", Email = "encoder@lgu.gov.ph", Mobile = "09171000001" },
                new { Id = 101, Username = "reviewer", RoleId = 5, FirstName = "Maria", LastName = "Santos", Email = "reviewer@lgu.gov.ph", Mobile = "09171000002" },
                new { Id = 102, Username = "assessor", RoleId = 6, FirstName = "Pedro", LastName = "Cruz", Email = "assessor@lgu.gov.ph", Mobile = "09171000003" },
                new { Id = 103, Username = "finalreviewer", RoleId = 7, FirstName = "Ana", LastName = "Reyes", Email = "finalreviewer@lgu.gov.ph", Mobile = "09171000004" },
                new { Id = 104, Username = "approver", RoleId = 8, FirstName = "Roberto", LastName = "Garcia", Email = "approver@lgu.gov.ph", Mobile = "09171000005" },
                new { Id = 105, Username = "mayor", RoleId = 9, FirstName = "Mayor", LastName = "Torres", Email = "mayor@lgu.gov.ph", Mobile = "09171000006" },
                new { Id = 106, Username = "sysadmin", RoleId = 10, FirstName = "Carl", LastName = "Rivera", Email = "sysadmin@lgu.gov.ph", Mobile = "09171000007" },
                new { Id = 107, Username = "releasingofficer", RoleId = 12, FirstName = "Liza", LastName = "Mendoza", Email = "releasingofficer@lgu.gov.ph", Mobile = "09171000008" },
            };

            foreach (var u in users)
            {
                modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Password = hashedPassword,
                        UserRoleId = u.RoleId,
                        UserProfileId = u.Id, // 1:1 with profile
                        LGUId = 1, // Consolacion
                        DepartmentId = null,
                        CreatedBy = createdBy,
                        CreatedAt = now
                    }
                );

                modelBuilder.Entity<UserProfile>().HasData(
                    new UserProfile
                    {
                        Id = u.Id,
                        UserId = u.Id,
                        FirstName = u.FirstName,
                        MiddleName = "",
                        LastName = u.LastName,
                        Email = u.Email,
                        MobileNo = u.Mobile,
                        CreatedBy = createdBy,
                        CreatedAt = now
                    }
                );
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
