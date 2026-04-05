using ePermitsApp.Entities;
using ePermitsApp.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ePermitsApp.Data.Seeders
{
    public class RequirementSeeder : BaseSeeder
    {
        public override int Order => 20;

        public override void Seed(ModelBuilder modelBuilder)
        {
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            const string createdBy = "System";

            modelBuilder.Entity<RequirementClassification>().HasData(
                new RequirementClassification
                {
                    Id = 1,
                    ReqClassDesc = "Building Permit Requirements",
                    ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit,
                    CreatedBy = createdBy,
                    CreatedAt = now
                },
                new RequirementClassification
                {
                    Id = 2,
                    ReqClassDesc = "Certificate of Occupancy Requirements",
                    ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy,
                    CreatedBy = createdBy,
                    CreatedAt = now
                }
            );

            modelBuilder.Entity<RequirementCategory>().HasData(
                new RequirementCategory { Id = 1, ReqClassId = 1, ReqCatDesc = "A. Application & Clearances", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 2, ReqClassId = 1, ReqCatDesc = "B. Property & Ownership Documents", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 3, ReqClassId = 1, ReqCatDesc = "C. Survey & Plans", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 4, ReqClassId = 1, ReqCatDesc = "D. Technical Documents", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 5, ReqClassId = 1, ReqCatDesc = "E. Proof of Payment", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 6, ReqClassId = 1, ReqCatDesc = "F. Additional Technical & Regulatory Requirements", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 7, ReqClassId = 1, ReqCatDesc = "G. Specialized Technical Requirements", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 8, ReqClassId = 2, ReqCatDesc = "Completion Documents", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 9, ReqClassId = 2, ReqCatDesc = "Inspection Certificates", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 10, ReqClassId = 2, ReqCatDesc = "Compliance Documents", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new RequirementCategory { Id = 11, ReqClassId = 2, ReqCatDesc = "Additional Requirements", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now }
            );

            modelBuilder.Entity<Requirement>().HasData(
                new Requirement { Id = 1, ReqCatId = 1, ReqDesc = "Building/Fencing Permit Application Form", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 2, ReqCatId = 1, ReqDesc = "Zoning Certification", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 3, ReqCatId = 1, ReqDesc = "Locational Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 4, ReqCatId = 1, ReqDesc = "Fire Safety Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 5, ReqCatId = 1, ReqDesc = "Barangay Construction Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 6, ReqCatId = 2, ReqDesc = "Tax Declaration (certified true copy)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 7, ReqCatId = 2, ReqDesc = "Tax Clearance / Tax Receipt", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 8, ReqCatId = 2, ReqDesc = "Certificate of Title (OCT/TCT)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 9, ReqCatId = 2, ReqDesc = "Affidavit of Consent (if applicable)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 10, ReqCatId = 2, ReqDesc = "Special Power of Attorney (if applicable)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 11, ReqCatId = 2, ReqDesc = "Residence certificate", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 12, ReqCatId = 3, ReqDesc = "Cadastral Survey Plan", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 13, ReqCatId = 3, ReqDesc = "Complete Building Plans (signed & sealed)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 14, ReqCatId = 4, ReqDesc = "Bill of Materials & Cost Estimates", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 15, ReqCatId = 4, ReqDesc = "General Specifications", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 16, ReqCatId = 5, ReqDesc = "Official Receipts (fees)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 17, ReqCatId = 6, ReqDesc = "Highway Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 18, ReqCatId = 6, ReqDesc = "Structural Analysis & Design", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 19, ReqCatId = 6, ReqDesc = "Height Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 20, ReqCatId = 6, ReqDesc = "Certificate of Safety Evaluation", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 21, ReqCatId = 6, ReqDesc = "ECC or CNC", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 22, ReqCatId = 6, ReqDesc = "DENR Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 23, ReqCatId = 6, ReqDesc = "SEC Registration (if corporation)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 24, ReqCatId = 6, ReqDesc = "Board Resolution / Secretary Certificate", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 25, ReqCatId = 6, ReqDesc = "Subdivision Approval", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 26, ReqCatId = 6, ReqDesc = "Homeowners Association Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 2, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 27, ReqCatId = 6, ReqDesc = "Highway Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 28, ReqCatId = 6, ReqDesc = "Structural Analysis & Design", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 29, ReqCatId = 6, ReqDesc = "Height Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 30, ReqCatId = 6, ReqDesc = "Certificate of Safety Evaluation", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 31, ReqCatId = 6, ReqDesc = "ECC or CNC", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 32, ReqCatId = 6, ReqDesc = "DENR Clearance", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 33, ReqCatId = 6, ReqDesc = "SEC Registration (if corporation)", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 34, ReqCatId = 6, ReqDesc = "Board Resolution / Secretary Certificate", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 35, ReqCatId = 6, ReqDesc = "Subdivision Approval", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 36, ReqCatId = 7, ReqDesc = "Field Density Test / Soil Test", ApplicationTypeScope = MaintenanceApplicationScopes.BuildingPermit, BuildingPermitCategoryId = 3, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 37, ReqCatId = 8, ReqDesc = "Copy of approved Building Permit", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 38, ReqCatId = 8, ReqDesc = "As-Built Plans (signed & sealed by professionals)", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 39, ReqCatId = 8, ReqDesc = "Construction completion certificate", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 40, ReqCatId = 8, ReqDesc = "Material test certificates (concrete, steel, etc.)", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 41, ReqCatId = 9, ReqDesc = "Fire Safety Inspection Certificate (FSIC)", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 42, ReqCatId = 9, ReqDesc = "Electrical Safety Inspection Certificate", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 43, ReqCatId = 9, ReqDesc = "Structural inspection report", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 44, ReqCatId = 9, ReqDesc = "Plumbing inspection certificate", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 45, ReqCatId = 9, ReqDesc = "Mechanical systems inspection (if applicable)", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 46, ReqCatId = 10, ReqDesc = "Environmental compliance monitoring report (if required)", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 47, ReqCatId = 10, ReqDesc = "Accessibility compliance certificate (for public buildings)", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 48, ReqCatId = 10, ReqDesc = "Waste management compliance certificate", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 49, ReqCatId = 10, ReqDesc = "Building maintenance plan", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },

                new Requirement { Id = 50, ReqCatId = 11, ReqDesc = "Updated tax declaration reflecting improvements", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 51, ReqCatId = 11, ReqDesc = "Insurance coverage for the structure", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now },
                new Requirement { Id = 52, ReqCatId = 11, ReqDesc = "Business permits (for commercial/industrial buildings)", ApplicationTypeScope = MaintenanceApplicationScopes.CertificateOfOccupancy, CreatedBy = createdBy, CreatedAt = now }
            );
        }
    }
}
