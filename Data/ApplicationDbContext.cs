using ePermitsApp.Entities;
using ePermits.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ePermitsApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        
        }

        public DbSet<Province> Provinces => Set<Province>();
        public DbSet<LGU> LGUs => Set<LGU>();
        public DbSet<Barangay> Barangays => Set<Barangay>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<RequirementClassification> RequirementClassifications => Set<RequirementClassification>();
        public DbSet<RequirementCategory> RequirementCategorys => Set<RequirementCategory>();
        public DbSet<Requirement> Requirements => Set<Requirement>();
        public DbSet<PermitApplicationType> PermitApplicationTypes => Set<PermitApplicationType>();
        public DbSet<OccupancyNature> OccupancyNatures => Set<OccupancyNature>();
        public DbSet<ProjectClassification> ProjectClassifications => Set<ProjectClassification>();
        public DbSet<ApplicantType> ApplicantTypes => Set<ApplicantType>();
        public DbSet<OwnershipType> OwnershipTypes => Set<OwnershipType>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

        public DbSet<Application> Applications { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Province>()
                .HasIndex(p => p.ProvinceName);

            modelBuilder.Entity<Province>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<LGU>()
                .HasIndex(l => new { l.ProvinceId, l.LGUName });
            
            modelBuilder.Entity<LGU>()
                .HasQueryFilter(l => !l.IsDeleted);

            modelBuilder.Entity<LGU>()
                .HasOne(l => l.Province)
                .WithMany(p => p.LGUs)
                .HasForeignKey(l => l.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Barangay>()
                .HasIndex(b => b.BarangayName);

            modelBuilder.Entity<Barangay>()
                .HasQueryFilter(b => !b.IsDeleted);                       

            modelBuilder.Entity<Barangay>()
                .Property(b => b.BarangayName)
                .IsRequired()
                .HasMaxLength(200);                      

            modelBuilder.Entity<Department>()
                .HasIndex(d => new { d.LGUId, d.DepartmentCode })
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasQueryFilter(d => !d.IsDeleted);

            modelBuilder.Entity<Department>()
                .Property(d => d.DepartmentCode)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Department>()
                .Property(d => d.DepartmentName)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<RequirementClassification>()
                .HasIndex(r => r.ReqClassDesc);

            modelBuilder.Entity<RequirementClassification>()
                .HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<RequirementCategory>()
                .HasIndex(r => new { r.ReqClassId, r.ReqCatDesc });

            modelBuilder.Entity<RequirementCategory>()
                .HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<RequirementCategory>()
                .HasOne(r => r.RequirementClassification)
                .WithMany(c => c.RequirementCategorys)
                .HasForeignKey(r => r.ReqClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Requirement>()
                .HasIndex(r => r.ReqDesc);

            modelBuilder.Entity<Requirement>()
                .HasQueryFilter(r => !r.IsDeleted);

            modelBuilder.Entity<Requirement>()
                .Property(r => r.ReqDesc)
                .IsRequired();

            modelBuilder.Entity<PermitApplicationType>()
                .HasIndex(p => p.PermitAppTypeDesc);

            modelBuilder.Entity<PermitApplicationType>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<OccupancyNature>()
                .HasIndex(o => o.OccupancyNatureDesc);

            modelBuilder.Entity<OccupancyNature>()
                .HasQueryFilter(o => !o.IsDeleted);

            modelBuilder.Entity<ProjectClassification>()
                .HasIndex(p => p.ProjectClassDesc);

            modelBuilder.Entity<ProjectClassification>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<ApplicantType>()
                .HasIndex(a => a.ApplicantTypeDesc);

            modelBuilder.Entity<ApplicantType>()
                .HasQueryFilter(a => !a.IsDeleted);

            modelBuilder.Entity<OwnershipType>()
                .HasIndex(o => o.OwnershipTypeDesc);

            modelBuilder.Entity<OwnershipType>()
                .HasQueryFilter(o => !o.IsDeleted);                       

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.UserRole)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.UserRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LGU)
                    .WithMany()
                    .HasForeignKey(e => e.LGUId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false); 

            });

            // UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserRoleDesc).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // UserProfile configuration
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MiddleName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MobileNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedBy).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Create unique index on Email
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasOne(e => e.User)
                    .WithOne(u => u.UserProfile)
                    .HasForeignKey<UserProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed default user roles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    UserRoleDesc = "admin",
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new UserRole
                {
                    Id = 2,
                    UserRoleDesc = "user",
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                },
                new UserRole
                {
                    Id = 3,
                    UserRoleDesc = "applicant",
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed provinces
            modelBuilder.Entity<Province>().HasData(
                new Province
                {
                    Id = 1,
                    ProvinceName = "Cebu",
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed LGUs
            modelBuilder.Entity<LGU>().HasData(
                new LGU
                {
                    Id = 1,
                    LGUName = "Consolacion",
                    ProvinceId = 1,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed departments
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    Id = 1,
                    DepartmentName = "Information Technology",
                    DepartmentCode = "IT",
                    LGUId = 1,
                    CreatedBy = "System",
                    CreatedAt = DateTime.UtcNow
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
