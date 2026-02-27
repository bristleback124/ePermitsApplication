using ePermitsApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class RequirementSeeder : BaseSeeder
    {
        public override int Order => 10;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            // --- Classifications ---
            // Id 1 = Building Permit Requirements
            // Id 2 = Certificate of Occupancy Requirements
            modelBuilder.Entity<RequirementClassification>().HasData(
                new RequirementClassification
                {
                    Id = 1,
                    ReqClassDesc = "Building Permit Requirements",
                    CreatedBy = createdBy,
                    CreatedAt = now
                },
                new RequirementClassification
                {
                    Id = 2,
                    ReqClassDesc = "Certificate of Occupancy Requirements",
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );

            // --- Categories ---
            // Building Permit categories (ClassId = 1)
            // Id 1 = Basic Documents
            // Id 2 = Technical Plans & Documents
            // Id 3 = Special Requirements
            // Certificate of Occupancy categories (ClassId = 2)
            // Id 4 = Completion Documents
            // Id 5 = Inspection Certificates
            // Id 6 = Compliance Documents
            // Id 7 = Additional Requirements
            modelBuilder.Entity<RequirementCategory>().HasData(
                new RequirementCategory { Id = 1, ReqCatDesc = "Basic Documents",            ReqClassId = 1, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 2, ReqCatDesc = "Technical Plans & Documents", ReqClassId = 1, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 3, ReqCatDesc = "Special Requirements",       ReqClassId = 1, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 4, ReqCatDesc = "Completion Documents",       ReqClassId = 2, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 5, ReqCatDesc = "Inspection Certificates",    ReqClassId = 2, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 6, ReqCatDesc = "Compliance Documents",       ReqClassId = 2, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 7, ReqCatDesc = "Additional Requirements",    ReqClassId = 2, CreatedBy = createdBy, CreatedAt = now }
            );

            // --- Requirements ---
            modelBuilder.Entity<Requirement>().HasData(
                // Basic Documents (CatId = 1)
                new Requirement { Id = 1,  ReqDesc = "Barangay Clearance",                                                      ReqCatId = 1, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 2,  ReqDesc = "Tax Declaration of Real Property",                                         ReqCatId = 1, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 3,  ReqDesc = "Latest Real Property Tax Receipt",                                         ReqCatId = 1, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 4,  ReqDesc = "Proof of ownership or right to build (TCT, deed of sale, lease contract)", ReqCatId = 1, CreatedBy = createdBy, CreatedAt = now },

                // Technical Plans & Documents (CatId = 2)
                new Requirement { Id = 5,  ReqDesc = "Architectural Plans (signed & sealed by Licensed Architect)",                      ReqCatId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 6,  ReqDesc = "Structural Plans (signed & sealed by Licensed Civil/Structural Engineer)",         ReqCatId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 7,  ReqDesc = "Electrical Plans (signed & sealed by Licensed Electrical Engineer)",               ReqCatId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 8,  ReqDesc = "Plumbing Plans (signed & sealed by Licensed Master Plumber)",                      ReqCatId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 9,  ReqDesc = "Mechanical Plans (if applicable, signed & sealed by Licensed Mechanical Engineer)", ReqCatId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 10, ReqDesc = "Bill of Materials/Specifications",                                                 ReqCatId = 2, CreatedBy = createdBy, CreatedAt = now },

                // Special Requirements (CatId = 3)
                new Requirement { Id = 11, ReqDesc = "DPWH clearance (for structures near national roads)",  ReqCatId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 12, ReqDesc = "CAAP clearance (for high-rise buildings)",             ReqCatId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 13, ReqDesc = "Environmental Compliance Certificate (ECC)",           ReqCatId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 14, ReqDesc = "Locational clearance from concerned agencies",         ReqCatId = 3, CreatedBy = createdBy, CreatedAt = now },

                // Completion Documents (CatId = 4)
                new Requirement { Id = 15, ReqDesc = "Copy of approved Building Permit",                   ReqCatId = 4, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 16, ReqDesc = "As-Built Plans (signed & sealed by professionals)",  ReqCatId = 4, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 17, ReqDesc = "Construction completion certificate",                ReqCatId = 4, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 18, ReqDesc = "Material test certificates (concrete, steel, etc.)", ReqCatId = 4, CreatedBy = createdBy, CreatedAt = now },

                // Inspection Certificates (CatId = 5)
                new Requirement { Id = 19, ReqDesc = "Fire Safety Inspection Certificate (FSIC)",      ReqCatId = 5, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 20, ReqDesc = "Electrical Safety Inspection Certificate",       ReqCatId = 5, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 21, ReqDesc = "Structural inspection report",                   ReqCatId = 5, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 22, ReqDesc = "Plumbing inspection certificate",                ReqCatId = 5, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 23, ReqDesc = "Mechanical systems inspection (if applicable)",  ReqCatId = 5, CreatedBy = createdBy, CreatedAt = now },

                // Compliance Documents (CatId = 6)
                new Requirement { Id = 24, ReqDesc = "Environmental compliance monitoring report (if required)",    ReqCatId = 6, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 25, ReqDesc = "Accessibility compliance certificate (for public buildings)", ReqCatId = 6, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 26, ReqDesc = "Waste management compliance certificate",                    ReqCatId = 6, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 27, ReqDesc = "Building maintenance plan",                                  ReqCatId = 6, CreatedBy = createdBy, CreatedAt = now },

                // Additional Requirements (CatId = 7)
                new Requirement { Id = 28, ReqDesc = "Updated tax declaration reflecting improvements",       ReqCatId = 7, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 29, ReqDesc = "Insurance coverage for the structure",                  ReqCatId = 7, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 30, ReqDesc = "Business permits (for commercial/industrial buildings)", ReqCatId = 7, CreatedBy = createdBy, CreatedAt = now }
            );
        }
    }
}
