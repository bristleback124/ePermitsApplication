using ePermitsApp.Entities;
using ePermitsApp.Entities.BuildingPermit;
using ePermits.Models;
using ePermitsApp.Entities.CoOApp;
using ePermitsApp.Data.Seeders;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<BuildingPermit> BuildingPermits => Set<BuildingPermit>();
        public DbSet<BuildingPermitAppInfo> BuildingPermitAppInfos => Set<BuildingPermitAppInfo>();
        public DbSet<BuildingPermitDesignProf> BuildingPermitDesignProfs => Set<BuildingPermitDesignProf>();
        public DbSet<BuildingPermitTechDoc> BuildingPermitTechDocs => Set<BuildingPermitTechDoc>();

        public DbSet<CoOApp> CoOApps => Set<CoOApp>();
        public DbSet<CoOAppProf> CoOAppProfs => Set<CoOAppProf>();
        public DbSet<CoOAppReqDoc> CoOAppReqDocs => Set<CoOAppReqDoc>();

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
                .HasIndex(d => d.DepartmentCode)
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
                .HasOne(r => r.RequirementCategory)
                .WithMany(c => c.Requirements)
                .HasForeignKey(r => r.ReqCatId)
                .OnDelete(DeleteBehavior.Restrict);

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

            // Application configuration
            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.BuildingPermit)
                    .WithOne(b => b.Application)
                    .HasForeignKey<BuildingPermit>(b => b.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CoOApp)
                    .WithOne(c => c.Application)
                    .HasForeignKey<CoOApp>(c => c.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    UserRoleDesc = "admin",
                    CreatedBy = "System",
                    CreatedAt = seedDate
                },
                new UserRole
                {
                    Id = 2,
                    UserRoleDesc = "user",
                    CreatedBy = "System",
                    CreatedAt = seedDate
                },
                new UserRole
                {
                    Id = 3,
                    UserRoleDesc = "applicant",
                    CreatedBy = "System",
                    CreatedAt = seedDate
                }
            );

            // BuildingPermit configuration
            modelBuilder.Entity<BuildingPermit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProjectTitle).IsRequired();
                entity.Property(e => e.PropertyAddLot).IsRequired();
                entity.Property(e => e.PropertyAddStreet).IsRequired();
                entity.Property(e => e.TCTNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TaxDeclarionNo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Coordinates).HasMaxLength(100);
                entity.Property(e => e.DigitalSignature).IsRequired();

                entity.Property(e => e.EstimatedCost).HasPrecision(18, 10);
                entity.Property(e => e.FloorAreaPerStorey).HasPrecision(18, 10);
                entity.Property(e => e.TotalFloorArea).HasPrecision(18, 10);
                entity.Property(e => e.ProjectScopeLotArea).HasPrecision(18, 10);
                entity.Property(e => e.PropertyDetailLotArea).HasPrecision(18, 10);

                entity.HasOne(e => e.AppInfo)
                    .WithOne(a => a.BuildingPermit)
                    .HasForeignKey<BuildingPermitAppInfo>(a => a.BuildingPermitId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.DesignProf)
                    .WithOne(d => d.BuildingPermit)
                    .HasForeignKey<BuildingPermitDesignProf>(d => d.BuildingPermitId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TechDoc)
                    .WithOne(t => t.BuildingPermit)
                    .HasForeignKey<BuildingPermitTechDoc>(t => t.BuildingPermitId)
                    .OnDelete(DeleteBehavior.Cascade);

                // FKs with navigation properties
                entity.HasOne(e => e.PermitApplicationType)
                    .WithMany()
                    .HasForeignKey(e => e.PermitAppTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.OccupancyNature)
                    .WithMany()
                    .HasForeignKey(e => e.OccupancyNatureId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProjectClassification)
                    .WithMany()
                    .HasForeignKey(e => e.ProjectClassId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Province)
                    .WithMany()
                    .HasForeignKey(e => e.ProvinceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LGU)
                    .WithMany()
                    .HasForeignKey(e => e.LGUId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Barangay)
                    .WithMany()
                    .HasForeignKey(e => e.BarangayId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BuildingPermitAppInfo configuration
            modelBuilder.Entity<BuildingPermitAppInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.ContactNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TIN).IsRequired().HasMaxLength(20);
                entity.Property(e => e.MailAddress).IsRequired();
                entity.Property(e => e.ReqDocProofOwnership).IsRequired();
                entity.Property(e => e.ReqDocBarangayClearance).IsRequired();
                entity.Property(e => e.ReqDocTaxDeclaration).IsRequired();
                entity.Property(e => e.ReqDocRealPropTaxReceipt).IsRequired();

                entity.HasOne<ApplicantType>()
                    .WithMany()
                    .HasForeignKey(e => e.ApplicantTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<OwnershipType>()
                    .WithMany()
                    .HasForeignKey(e => e.OwnershipTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BuildingPermitDesignProf configuration
            modelBuilder.Entity<BuildingPermitDesignProf>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IoCFullName).IsRequired();
                entity.Property(e => e.IoCPRCNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.IoCPTRNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SEFullName).IsRequired();
                entity.Property(e => e.SEPRCNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SEPTRNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.EEFullName).IsRequired();
                entity.Property(e => e.EEPRCNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.EEPTRNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SPEFullName).IsRequired();
                entity.Property(e => e.SPEPRCNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SPEPTRNo).IsRequired().HasMaxLength(20);

                entity.Property(e => e.MEPRCNo).HasMaxLength(20);
                entity.Property(e => e.MEPTRNo).HasMaxLength(20);
                entity.Property(e => e.ECEPRCNo).HasMaxLength(20);
                entity.Property(e => e.ECEPTRNo).HasMaxLength(20);
                entity.Property(e => e.ContractorPCABNo).HasMaxLength(20);
                entity.Property(e => e.ContractorClassification).HasMaxLength(20);
            });

            // BuildingPermitTechDoc configuration
            modelBuilder.Entity<BuildingPermitTechDoc>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TechDocIoCPlans).IsRequired();
                entity.Property(e => e.TechDocSEPlans).IsRequired();
                entity.Property(e => e.TechDocEEPlans).IsRequired();
                entity.Property(e => e.TechDocSPPlans).IsRequired();
                entity.Property(e => e.TechDocBOMCost).IsRequired();
                entity.Property(e => e.TechDocSoW).IsRequired();
            });

            // CoOApp configuration
            modelBuilder.Entity<CoOApp>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BldgPermitNo).IsRequired();
                entity.Property(e => e.ProjectTitle).IsRequired();
                entity.Property(e => e.ProjLocLot).IsRequired();
                entity.Property(e => e.ProjLocStreet).IsRequired();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.ContactNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TIN).IsRequired().HasMaxLength(20);
                entity.Property(e => e.MailAddress).IsRequired();
                entity.Property(e => e.DigitalSignature).IsRequired();

                entity.Property(e => e.FloorArea).HasPrecision(18, 10);

                entity.HasOne(e => e.CoOAppProf)
                    .WithOne(p => p.CoOApp)
                    .HasForeignKey<CoOAppProf>(p => p.CoOAppId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CoOAppReqDoc)
                    .WithOne(r => r.CoOApp)
                    .HasForeignKey<CoOAppReqDoc>(r => r.CoOAppId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Province)
                    .WithMany()
                    .HasForeignKey(e => e.ProvinceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Lgu)
                    .WithMany()
                    .HasForeignKey(e => e.LGUId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Barangay)
                    .WithMany()
                    .HasForeignKey(e => e.BarangayId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.OccupancyNature)
                    .WithMany()
                    .HasForeignKey(e => e.OccupancyNatureId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ApplicantType)
                    .WithMany()
                    .HasForeignKey(e => e.ApplicantTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CoOAppProf configuration
            modelBuilder.Entity<CoOAppProf>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IoCFullName).IsRequired();
                entity.Property(e => e.IoCPRCNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.IoCPTRNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.EoRFullName).IsRequired();
                entity.Property(e => e.EoRPRCorPTRNo).IsRequired().HasMaxLength(20);
                entity.Property(e => e.EoRSpecialization).IsRequired();
            });

            // CoOAppReqDoc configuration
            modelBuilder.Entity<CoOAppReqDoc>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReqDocBldgPermitSPlans).IsRequired();
                entity.Property(e => e.ReqDocAsBuiltPlans).IsRequired();
                entity.Property(e => e.ReqDocConsLogbook).IsRequired();
                entity.Property(e => e.ReqDocConsPhotos).IsRequired();
                entity.Property(e => e.ReqDocBrgyClearance).IsRequired();
                entity.Property(e => e.ReqDocFSIC).IsRequired();
            });

            // Run modular seeders (HasData calls)
            DatabaseSeederRunner.RunSeeders(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
