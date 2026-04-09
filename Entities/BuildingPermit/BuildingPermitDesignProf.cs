using ePermitsApp.Entities.Common;

namespace ePermitsApp.Entities.BuildingPermit
{
    public class BuildingPermitDesignProf : BaseEntity
    {
        public int Id { get; set; }
        public int BuildingPermitId { get; set; }

        // In-charge of Construction 
        public string IoCFullName { get; set; } = string.Empty;
        public string IoCPRCNo { get; set; } = string.Empty;
        public string IoCPTRNo { get; set; } = string.Empty;
        public DateTime IOCValidity { get; set; }

        // Structural Engineer 
        public string SEFullName { get; set; } = string.Empty;
        public string SEPRCNo { get; set; } = string.Empty;
        public string SEPTRNo { get; set; } = string.Empty;
        public DateTime SEValidity { get; set; }

        // Electrical Engineer 
        public string EEFullName { get; set; } = string.Empty;
        public string EEPRCNo { get; set; } = string.Empty;
        public string EEPTRNo { get; set; } = string.Empty;
        public DateTime EEValidity { get; set; }

        // Sanitary/Plumbing Engineer 
        public string SPEFullName { get; set; } = string.Empty;
        public string SPEPRCNo { get; set; } = string.Empty;
        public string SPEPTRNo { get; set; } = string.Empty;
        public DateTime SPEValidity { get; set; }

        // Mechanical Engineer 
        public string? MEFullName { get; set; }
        public string? MEPRCNo { get; set; }
        public string? MEPTRNo { get; set; }
        public DateTime? MEValidity { get; set; }

        // Geotechnical / Soil Engineer
        public string? GSEFullName { get; set; }
        public string? GSEPRCNo { get; set; }
        public string? GSEPTRNo { get; set; }
        public DateTime? GSEValidity { get; set; }

        // Electronics & Comm. Engineer 
        public string? ECEFullName { get; set; }
        public string? ECEPRCNo { get; set; }
        public string? ECEPTRNo { get; set; }
        public DateTime? ECEValidity { get; set; }

        // Contractor 
        public string? ContractorBusinessName { get; set; }
        public string? ContractorPCABNo { get; set; }
        public string? ContractorClassification { get; set; }
        public DateTime? ContractorValidity { get; set; }
        
        public BuildingPermit? BuildingPermit { get; set; }
    }
}
